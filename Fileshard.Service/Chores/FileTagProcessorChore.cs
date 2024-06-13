using Fileshard.Service.Repository;
using Fileshard.Service.Structs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Fileshard.Service.Chores
{
    public class FileTagProcessorChore : IFileshardChore<FileshardObject>
    {
        private ICollectionRepository _collectionRepository;
        private Guid _collectionId;
        private String _endpoint;

        public FileTagProcessorChore(ICollectionRepository collectionRepository, Guid collectionId, String endpoint)
        {
            _collectionRepository = collectionRepository;
            _collectionId = collectionId;
            _endpoint = endpoint;
        }

        public async Task<ProgressIterator<FileshardObject>> GetTask()
        {
            var objects = await _collectionRepository.GetObjects(_collectionId);
            /*var filteredObjects = objects.Where(e => e.Files.Any(f => f.Metas.Count < 2));*/

            return new Iterator(_collectionRepository, objects, _endpoint);
        }


        public record class TagResponse
        {
            [JsonPropertyName("ratio")]
            public Dictionary<String, float> RatioTags { get; set; }
        }

        public record class TagRequest (
            string? path = null
        );

        private class Iterator : ProgressIterator<FileshardObject>
        {
            private ICollectionRepository _collectionRepository;
            private String _endpoint;

            private HttpClient _sharedClient;

            public Iterator(ICollectionRepository collectionRepository, IEnumerable<FileshardObject> collection, String endpoint) : base(collection)
            {
                _collectionRepository = collectionRepository;
                _endpoint = endpoint;

                try { 
                    _sharedClient = new()
                    {
                        BaseAddress = new Uri(endpoint),
                    };
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                this.ThreadCount = 1;
            }

            private Exception? _lastException;

            protected override string BuildStatusText(int processedItems, int totalItems, float progress)
            {
                if (_lastException != null)
                {
                    return $"Error: {_lastException.Message}";
                }

                return $"Progress: {progress:F2}% ({processedItems}/{totalItems} items)";
            }

            protected override async Task ProcessItemAsync(FileshardObject item, CancellationToken token)
            {
                try { 
                    foreach (var file in item.Files) { 
                        // TODO Could filter namespace by the first query
                        if (await _collectionRepository.ObjectHasNamespaceAlready(item.Id, "default"))
                        {
                            return;
                        }

                        using HttpResponseMessage response = await _sharedClient.PostAsJsonAsync("", new TagRequest(path: file.InternalPath));

                        if (response.IsSuccessStatusCode)
                        {
                            TagResponse? tagResponse = await response.Content.ReadFromJsonAsync<TagResponse>(cancellationToken: token);

                            if (tagResponse == null)
                            {
                                throw new Exception("Server doesn't return tags properly");
                            }

                            foreach (var tag in tagResponse.RatioTags)
                            {
                                await _collectionRepository.UpsertObjectTag("default", tag.Key, tag.Value, item.Id);
                            }
                        }
                        else
                        {
                            throw new Exception("Failed to process tags");
                        }

                        _lastException = null;
                    }
                } catch (Exception e)
                {
                    _lastException = e;
                }
            }
        }
    }
}

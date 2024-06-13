using CoenM.ImageHash.HashAlgorithms;
using CoenM.ImageHash;
using Fileshard.Frontend.Helpers;
using Fileshard.Service.Repository;
using Fileshard.Service.Structs;
using ImageMagick;

namespace Fileshard.Service.Chores
{
    public class FileProcessorChore : IFileshardChore<FileshardObject>
    {
        private ICollectionRepository _collectionRepository;
        private Guid _collectionId;

        public FileProcessorChore(ICollectionRepository collectionRepository, Guid collectionId)
        {
            _collectionRepository = collectionRepository;
            _collectionId = collectionId;
        }

        public async Task<ProgressIterator<FileshardObject>> GetTask()
        {
            var objects = await _collectionRepository.GetObjects(_collectionId);
            /*var filteredObjects = objects.Where(e => e.Files.Any(f => f.Metas.Count < 2));*/

            return new Iterator(_collectionRepository, objects);
        }

        private class Iterator : ProgressIterator<FileshardObject>
        {
            private ICollectionRepository _collectionRepository;

            public Iterator(ICollectionRepository collectionRepository, IEnumerable<FileshardObject> collection) : base(collection) {
                _collectionRepository = collectionRepository;
                this.ThreadCount = 1;
            }

            protected override async Task ProcessItemAsync(FileshardObject item, CancellationToken token)
            {
                foreach (var file in item.Files)
                {
                    if (!_collectionRepository.FileHasMeta("hash:md5", file.Id).Result) { 
                        try
                        {
                            String hash = HashUtil.ComputeMD5(file.InternalPath);
                            await _collectionRepository.UpsertMeta("hash:md5", hash, file.Id);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    try
                    {
                        if (!_collectionRepository.FileHasMetas(file.Id, "image:width", "image:height", "image:format").Result) { 
                            // Using Magick .NET to read image dimensions
                            using (var image = new MagickImage(file.InternalPath))
                            {
                                await _collectionRepository.UpsertMeta("image:width", (long) image.Width, file.Id);
                                await _collectionRepository.UpsertMeta("image:height", (long) image.Height, file.Id);

                                await _collectionRepository.UpsertMeta("image:format", image.Format.ToString(), file.Id);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    // Read file's Date Created and Date Modified
                    try
                    {
                        if (!_collectionRepository.FileHasMetas(file.Id, "date:created", "date:modified").Result) { 
                            var dateCreated = File.GetCreationTime(file.InternalPath);
                            var dateModified = File.GetLastWriteTime(file.InternalPath);

                            await _collectionRepository.UpsertMeta("date:created", dateCreated, file.Id);
                            await _collectionRepository.UpsertMeta("date:modified", dateModified, file.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    try
                    {
                        if (!_collectionRepository.FileHasMeta("hash:ImageHash:diff", file.Id).Result) { 
                            var diffHash = new DifferenceHash();
                            ulong hash = 0;
                            using (var fileStream = File.OpenRead(file.InternalPath))
                            {
                                hash = diffHash.Hash(fileStream);
                            }

                            if (diffHash == null || hash == 0) continue;

                            await _collectionRepository.UpsertMeta("hash:ImageHash:diff", (long) hash, file.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                return;
            }
        }
    }
}

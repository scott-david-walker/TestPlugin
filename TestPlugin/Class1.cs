using KlaarDesk.Plugins;
using Minio;
using Minio.DataModel.Args;
namespace TestPlugin;

public class FakePlugin : IStoragePlugin
{
    private const string BucketName = "klaardesk-test-bucket-2";
    public string Name { get; } = "Fake Slack Plugin";
    private IMinioClient _minio;
    public Dictionary<string, string> ConfigurationFields()
    {
        return new()
        {
            { "api_url", "Api Url" },
            { "access_key", "Access Key" },
            { "secret_key", "Secret Key" },
        };
    }

    public async Task InitialisePlugin(List<KeyValuePair<string, string>> configurationFields)
    {
        var apiUrl = configurationFields.FirstOrDefault(x => x.Key == "api_url");
        var accessKey = configurationFields.FirstOrDefault(x => x.Key == "access_key");
        var secretKey = configurationFields.FirstOrDefault(x => x.Key == "secret_key");

        if (apiUrl.Value == null)
        {
            throw new ArgumentException("Api Url is required");
        }
        
        if (accessKey.Value == null)
        {
            throw new ArgumentException("Access Key is required");
        }
        
        if (secretKey.Value == null)
        {
            throw new ArgumentException("Secret Key is required");
        }
        
        _minio = new MinioClient()
            .WithEndpoint(apiUrl.Value)
            .WithCredentials(accessKey.Value, secretKey.Value)
            .WithSSL(false)
            .Build();
         
        var beArgs = new BucketExistsArgs()
            .WithBucket(BucketName);
       
        var found = await _minio.BucketExistsAsync(beArgs);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(BucketName);
            await _minio.MakeBucketAsync(mbArgs);
        }
    }
    public async Task SaveAsync(string id, MemoryStream data)
    {
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(id)
            .WithObjectSize(data.Length)
            .WithStreamData(data);
        await _minio.PutObjectAsync(putObjectArgs);
    }

    public async Task<MemoryStream> Retrieve(string id)
    {
        var memoryStream = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(BucketName)
            .WithObject(id)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0; 
            });
        _ = await _minio.GetObjectAsync(args);
        return memoryStream;
    }
}
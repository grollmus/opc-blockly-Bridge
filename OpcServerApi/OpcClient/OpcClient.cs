using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OpcServerApi.DTO;
using StatusCodes = Opc.Ua.StatusCodes;

namespace OpcServerApi.OpcClient;

public class OpcClient : IOpcClient
{
    const string EndpointUrl = "opc.tcp://172.16.0.1:4840";

    public async Task<bool> Read(GetValueDto getValueDto)
    {
        var session = await OpenSession(EndpointUrl);

        var root = session.NodeCache.Find(Objects.RootFolder);

        var readValue = await session.ReadValueAsync(getValueDto.NodeId);

        if (readValue.StatusCode != StatusCodes.Good)
            Console.WriteLine($"Could not read {getValueDto.NodeId}");

        Console.WriteLine($"Read successful, Value: {readValue.Value}");

        await session.CloseAsync();

        return (bool)readValue.Value;
    }

    public async Task<bool> Write(PostValueDto newValue)
    {
        var session = await OpenSession(EndpointUrl);

        var root = session.NodeCache.Find(Objects.RootFolder);

        var writeValue = new WriteValue
        {
            NodeId = newValue.NodeId,
            AttributeId = Attributes.Value,
            Value = new DataValue(new Variant(newValue.Value))
        };

        var result = await session.WriteAsync(null, new[] { writeValue }, default);

        if (result.Results.All(StatusCode.IsGood))
            return true;

        Console.WriteLine("Error while writing data");
        return false;
    }

    private static async Task<Session> OpenSession(string endpointUrl)
    {
        var endpoint = new ConfiguredEndpoint(null, new EndpointDescription(endpointUrl));
        var application = new ApplicationInstance
        {
            ApplicationName = "UA Sample Client",
            ApplicationType = ApplicationType.Client,
            ConfigSectionName = "OpcClient"
        };
        try
        {
            var applicationConfiguration = await application.LoadApplicationConfiguration(false);
            var session = Session.Create(applicationConfiguration, endpoint, true, "YourSessionName", 60000, null, null)
                .Result;

            return session;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
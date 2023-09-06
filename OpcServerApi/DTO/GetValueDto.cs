namespace OpcServerApi.DTO;

public class GetValueDto
{
    public GetValueDto(string nodeId)
    {
        NodeId = nodeId;
    }

    public string NodeId { get; set; }
}
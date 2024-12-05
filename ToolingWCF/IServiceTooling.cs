using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using ToolingLib;
using ToolingLib.Models;

namespace ToolingWCF
{
    [ServiceContract]
    public interface IServiceTooling
    {
        #region Public Methods

        [OperationContract]
        [WebGet(UriTemplate = "/AddTool?PressId={PressId}&Width={Width}&Position={Position}&MagazineID={MagazineID}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse AddTool(int PressId, int Width, double Position, int MagazineID);

        [OperationContract]
        [WebGet(UriTemplate = "/AddToolFirstFreePosition?PressId={PressId}&Width={Width}&MagazineID={MagazineID}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse AddToolFirstFreePosition(int PressId, int Width, int MagazineID);

        [OperationContract]
        [WebGet(UriTemplate = "/GetStatusMagazine?MagazineID={MagazineID}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatMT GetStatusMagazine(int MagazineID);

        [OperationContract]
        [WebGet(UriTemplate = "/GetStatusMagazines", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatMT GetStatusMagazines();

        [OperationContract]
        [WebGet(UriTemplate = "/GetStatusPress?PressId={PressId}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatTP GetStatusPress(int PressId);

        [OperationContract]
        [WebGet(UriTemplate = "/LoadMagazine?FileName={FileName}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse LoadMagazine(string FileName);

        [OperationContract]
        [WebGet(UriTemplate = "/LoadRecipe?PressId={PressId}&Name={RecipeName}&Source={RecipeSource}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse LoadRecipe(int PressId, string RecipeName, string RecipeSource);

        [OperationContract]
        [WebGet(UriTemplate = "/RemoveTool?PressId={PressId}&Width={Width}&Position={Position}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse RemoveTool(int PressId, int Width, double Position);

        [OperationContract]
        [WebGet(UriTemplate = "/RemoveAllTools?PressId={PressId}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse RemoveAllTools(int PressId);

        [OperationContract]
        [WebGet(UriTemplate = "/Validation", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse Validation();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/NewRecipe", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ResponseFormatElse NewRecipeFile(DataRequest data);

        [OperationContract]
        [WebGet(UriTemplate = "/GetRecipes", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<RecipeModel> GetRecipes();

        [OperationContract]
        [WebGet(UriTemplate = "/GetPressBars", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<PressBar> GetPressBars();

        [OperationContract]
        [WebGet(UriTemplate = "/GetMagazines", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        int[] GetMagazines();

        [OperationContract]
        [WebGet(UriTemplate = "/SaveBarAsRecipe?PressId={PressId}&Format={format}&Name={name}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse SaveBarAsRecipe(int PressId, string format, string name);

        [OperationContract]
        [WebGet(UriTemplate = "/DeleteRecipe?Format={format}&Name={name}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        ResponseFormatElse DeleteRecipe(string format, string name);
    }

    #endregion Public Methods
}

[System.SerializableAttribute()]
[DataContract]
public class ResponseFormatElse
{
    #region Properties

    [DataMember(Order = 2)]
    public object Data { get; set; }

    [DataMember(Order = 1)]
    public string Message { get; set; }

    [DataMember(Order = 0)]
    public int Status { get; set; }

    #endregion Properties
}

[System.SerializableAttribute()]
[DataContract]
public class ResponseFormatMT
{
    #region Properties

    [DataMember(Order = 2)]
    public IEnumerable<MagazineTool> Data { get; set; }

    [DataMember(Order = 1)]
    public string Message { get; set; }

    [DataMember(Order = 0)]
    public int Status { get; set; }

    #endregion Properties
}

[System.SerializableAttribute()]
[DataContract]
public class ResponseFormatTP
{
    #region Properties

    [DataMember(Order = 2)]
    public IEnumerable<ToolPress> Data { get; set; }

    [DataMember(Order = 1)]
    public string Message { get; set; }

    [DataMember(Order = 0)]
    public int Status { get; set; }

    #endregion Properties
}

[System.SerializableAttribute()]
[DataContract]
public class DataRequest
{
    [DataMember]
    public string Json { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Source { get; set; }
}
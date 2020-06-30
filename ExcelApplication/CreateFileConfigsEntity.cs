
//===================================================
//作    者：Lixi  
//创建时间：2019-05-30 11:35:11
//===================================================
using System.Collections;

/// <summary>
/// CreateConfigs实体
/// </summary>
public partial class CreateFileConfigsEntity
{

    public string id;
    /// <summary>
    /// 实体基类名称
    /// </summary>
    public string entityBaseName;

    /// <summary>
    /// 数据访问基类名称
    /// </summary>
    public string dbModelBaseName;

    /// <summary>
    /// 是否生成读取Xml方法
    /// </summary>
    public bool isCreateReadXmlFun;

    /// <summary>
    /// 是否生成读取Json方法
    /// </summary>
    public bool isCreateReadJsonFun;

    /// <summary>
    /// 是否生成Xml文件
    /// </summary>
    public bool isCreateXmlFile;

    /// <summary>
    /// 是否生成json文件
    /// </summary>
    public bool isCreateJsonFile;

    /// <summary>
    /// 是否生成实体文件
    /// </summary>
    public bool isCreateEntityFile;

    /// <summary>
    /// 是否生成数据访问文件
    /// </summary>
    public bool isCreateDBModelFile;

    /// <summary>
    /// 是否生成二进制文件
    /// </summary>
    public bool isCreateByteFile;

  public CreateFileConfigsEntity XmlElementToObject(System.Xml.XmlElement element)
  {
     id= element.GetAttribute("id");
     entityBaseName= element.GetAttribute("entityBaseName");
     dbModelBaseName= element.GetAttribute("dbModelBaseName");
     isCreateReadXmlFun= bool.Parse( element.GetAttribute("isCreateReadXmlFun"));
     isCreateReadJsonFun= bool.Parse( element.GetAttribute("isCreateReadJsonFun"));
     isCreateXmlFile= bool.Parse( element.GetAttribute("isCreateXmlFile"));
     isCreateJsonFile= bool.Parse( element.GetAttribute("isCreateJsonFile"));
     isCreateEntityFile= bool.Parse( element.GetAttribute("isCreateEntityFile"));
     isCreateDBModelFile= bool.Parse( element.GetAttribute("isCreateDBModelFile"));
     isCreateByteFile= bool.Parse( element.GetAttribute("isCreateByteFile"));
     return this;
  }
}

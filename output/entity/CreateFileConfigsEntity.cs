
//===================================================
//作    者：Lixi  
//创建时间：2020-06-30 09:53:18
//===================================================
using System.Collections;

/// <summary>
/// CreateFileConfigs实体
/// </summary>
public partial class CreateFileConfigsEntity : DataTableEntityBase
{
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

  public CreateFileConfigsEntity XmlToObject(System.Xml.XmlElement element)
  {
	 if (element.HasAttribute("id") && !string.IsNullOrEmpty(element.GetAttribute("id")))
	 {
		id= element.GetAttribute("id");
	 }

	 if (element.HasAttribute("entityBaseName") && !string.IsNullOrEmpty(element.GetAttribute("entityBaseName")))
	 {
		entityBaseName= element.GetAttribute("entityBaseName");
	 }

	 if (element.HasAttribute("dbModelBaseName") && !string.IsNullOrEmpty(element.GetAttribute("dbModelBaseName")))
	 {
		dbModelBaseName= element.GetAttribute("dbModelBaseName");
	 }

	 if (element.HasAttribute("isCreateReadXmlFun") && !string.IsNullOrEmpty(element.GetAttribute("isCreateReadXmlFun")))
	 {
		isCreateReadXmlFun= bool.Parse( element.GetAttribute("isCreateReadXmlFun"));
	 }

	 if (element.HasAttribute("isCreateReadJsonFun") && !string.IsNullOrEmpty(element.GetAttribute("isCreateReadJsonFun")))
	 {
		isCreateReadJsonFun= bool.Parse( element.GetAttribute("isCreateReadJsonFun"));
	 }

	 if (element.HasAttribute("isCreateXmlFile") && !string.IsNullOrEmpty(element.GetAttribute("isCreateXmlFile")))
	 {
		isCreateXmlFile= bool.Parse( element.GetAttribute("isCreateXmlFile"));
	 }

	 if (element.HasAttribute("isCreateJsonFile") && !string.IsNullOrEmpty(element.GetAttribute("isCreateJsonFile")))
	 {
		isCreateJsonFile= bool.Parse( element.GetAttribute("isCreateJsonFile"));
	 }

	 if (element.HasAttribute("isCreateEntityFile") && !string.IsNullOrEmpty(element.GetAttribute("isCreateEntityFile")))
	 {
		isCreateEntityFile= bool.Parse( element.GetAttribute("isCreateEntityFile"));
	 }

	 if (element.HasAttribute("isCreateDBModelFile") && !string.IsNullOrEmpty(element.GetAttribute("isCreateDBModelFile")))
	 {
		isCreateDBModelFile= bool.Parse( element.GetAttribute("isCreateDBModelFile"));
	 }

	 if (element.HasAttribute("isCreateByteFile") && !string.IsNullOrEmpty(element.GetAttribute("isCreateByteFile")))
	 {
		isCreateByteFile= bool.Parse( element.GetAttribute("isCreateByteFile"));
	 }

     return this;
  }
}

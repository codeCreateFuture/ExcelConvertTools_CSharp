
//===================================================
//作    者：Lixi  
//创建时间：2020-06-30 09:53:17
//===================================================
using System.Collections;

/// <summary>
/// comptConfig实体
/// </summary>
public partial class comptConfigEntity : DataTableEntityBase
{
    /// <summary>
    /// 组件英文名称
    /// </summary>
    public string english;

    /// <summary>
    /// 中文名称
    /// </summary>
    public string china;

    /// <summary>
    /// 类型
    /// </summary>
    public string type;

  public comptConfigEntity XmlToObject(System.Xml.XmlElement element)
  {
	 if (element.HasAttribute("id") && !string.IsNullOrEmpty(element.GetAttribute("id")))
	 {
		id= element.GetAttribute("id");
	 }

	 if (element.HasAttribute("english") && !string.IsNullOrEmpty(element.GetAttribute("english")))
	 {
		english= element.GetAttribute("english");
	 }

	 if (element.HasAttribute("china") && !string.IsNullOrEmpty(element.GetAttribute("china")))
	 {
		china= element.GetAttribute("china");
	 }

	 if (element.HasAttribute("type") && !string.IsNullOrEmpty(element.GetAttribute("type")))
	 {
		type= element.GetAttribute("type");
	 }

     return this;
  }
}

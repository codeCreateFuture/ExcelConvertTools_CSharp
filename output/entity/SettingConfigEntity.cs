
//===================================================
//作    者：Lixi  
//创建时间：2020-06-30 09:53:19
//===================================================
using System.Collections;

/// <summary>
/// SettingConfig实体
/// </summary>
public partial class SettingConfigEntity : DataTableEntityBase
{
    /// <summary>
    /// 关键字
    /// </summary>
    public string key;

    /// <summary>
    /// 值
    /// </summary>
    public string value;

    /// <summary>
    /// 描述
    /// </summary>
    public string desc;

  public SettingConfigEntity XmlToObject(System.Xml.XmlElement element)
  {
	 if (element.HasAttribute("id") && !string.IsNullOrEmpty(element.GetAttribute("id")))
	 {
		id= element.GetAttribute("id");
	 }

	 if (element.HasAttribute("key") && !string.IsNullOrEmpty(element.GetAttribute("key")))
	 {
		key= element.GetAttribute("key");
	 }

	 if (element.HasAttribute("value") && !string.IsNullOrEmpty(element.GetAttribute("value")))
	 {
		value= element.GetAttribute("value");
	 }

	 if (element.HasAttribute("desc") && !string.IsNullOrEmpty(element.GetAttribute("desc")))
	 {
		desc= element.GetAttribute("desc");
	 }

     return this;
  }
}

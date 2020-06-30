
//===================================================
//作    者：Lixi  
//创建时间：2019-06-11 18:24:09
//===================================================
using System.Collections;

/// <summary>
/// SettingConfig实体
/// </summary>
public partial class SettingConfigEntity
{

    public string Id;
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
     Id= element.GetAttribute("Id");
     key= element.GetAttribute("key");
     value= element.GetAttribute("value");
     desc= element.GetAttribute("desc");
     return this;
  }
}

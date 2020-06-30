
//===================================================
//作    者：Lixi  
//创建时间：2020-06-30 09:53:18
//===================================================
using System.Collections;

/// <summary>
/// GradeStudent实体
/// </summary>
public partial class GradeStudentEntity : DataTableEntityBase
{
    /// <summary>
    /// 班级
    /// </summary>
    public string grade;

    /// <summary>
    /// 学号
    /// </summary>
    public string studentId;

    /// <summary>
    /// 学生姓名
    /// </summary>
    public string studentName;

  public GradeStudentEntity XmlToObject(System.Xml.XmlElement element)
  {
	 if (element.HasAttribute("id") && !string.IsNullOrEmpty(element.GetAttribute("id")))
	 {
		id= element.GetAttribute("id");
	 }

	 if (element.HasAttribute("grade") && !string.IsNullOrEmpty(element.GetAttribute("grade")))
	 {
		grade= element.GetAttribute("grade");
	 }

	 if (element.HasAttribute("studentId") && !string.IsNullOrEmpty(element.GetAttribute("studentId")))
	 {
		studentId= element.GetAttribute("studentId");
	 }

	 if (element.HasAttribute("studentName") && !string.IsNullOrEmpty(element.GetAttribute("studentName")))
	 {
		studentName= element.GetAttribute("studentName");
	 }

     return this;
  }
}

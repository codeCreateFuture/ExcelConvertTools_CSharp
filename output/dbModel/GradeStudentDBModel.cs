
//===================================================
//作    者：Lixi  
//创建时间：2020-06-30 09:53:18
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;

/// <summary>
/// GradeStudent数据管理
/// </summary>
public partial class GradeStudentDBModel : DataTableDBModelBase<GradeStudentDBModel, GradeStudentEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "GradeStudent"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(string dataTableData)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(dataTableData);
        XmlElement root = doc.DocumentElement;

        foreach (XmlElement node in root)
        {
            GradeStudentEntity entity = new GradeStudentEntity();
            entity = entity.XmlToObject(node);

            m_List.Add(entity);
            m_Dic[entity.id] = entity;
        }
    }
}
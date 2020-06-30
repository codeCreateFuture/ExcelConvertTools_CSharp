
//===================================================
//作    者：Lixi  
//创建时间：2020-06-30 09:53:19
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;

/// <summary>
/// SettingConfig数据管理
/// </summary>
public partial class SettingConfigDBModel : DataTableDBModelBase<SettingConfigDBModel, SettingConfigEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "SettingConfig"; } }

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
            SettingConfigEntity entity = new SettingConfigEntity();
            entity = entity.XmlToObject(node);

            m_List.Add(entity);
            m_Dic[entity.id] = entity;
        }
    }
}
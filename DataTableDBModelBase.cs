using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 数据表管理基类
/// </summary>
public abstract class DataTableDBModelBase<T, P>
where T : class, new()
where P : DataTableEntityBase
{
    protected List<P> m_List;
    protected Dictionary<string, P> m_Dic;

    public DataTableDBModelBase()
    {
        m_List = new List<P>();
        m_Dic = new Dictionary<string, P>();
        LoadData();
    }

    #region 单例
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
    #endregion

    /// <summary>
    /// 文件路径,重写函数定制专属路径
    /// </summary>
    protected virtual string FilePath { get { return Application.dataPath; } }

    #region 需要子类实现的属性或方法
    /// <summary>
    /// 数据表名
    /// </summary>
    public abstract string DataTableName { get; }

    /// <summary>
    /// 加载表格数据，子类重写加载数据表格(配置文件)方法
    /// </summary>
    /// <param name="parse"></param>
    /// <returns></returns>
    // protected abstract void LoadList(MMO_MemoryStream ms);
    protected abstract void LoadList(string dataTableData);
    #endregion


    #region 加载数据表数据 LoadData
    /// <summary>
    /// 加载数据表数据
    /// </summary>
    public void LoadData()
    {
        string configFullPath = string.Format(FilePath + "/{1}.xml", FilePath, DataTableName);
        if (File.Exists(configFullPath))
        {
            string data = "";
            using (FileStream fs = new FileStream(configFullPath, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    data = sr.ReadToEnd();
                }
            }
            if(string.IsNullOrEmpty(data))
            {
                Debug.LogError(DataTableName+"文件的内容为空 ：" + configFullPath);
            }
            LoadList(data);
        }
        else
        {
            Debug.LogError("不存在这样的文件 ：" + configFullPath);
        }
    }
    #endregion

    #region GetList 获取集合
    /// <summary>
    /// 获取集合
    /// </summary>
    /// <returns></returns>
    public List<P> GetList()
    {
        return m_List;
    }
    #endregion

    #region Get 根据编号获取实体
    /// <summary>
    /// 根据编号获取实体
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public P Get(string id)
    {
        if (m_Dic.ContainsKey(id))
        {
            return m_Dic[id];
        }
        return null;
    }
    #endregion

    public void Clear()
    {
        m_List.Clear();
        m_Dic.Clear();
    }
}

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AE_ConnectGDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 获取SDE图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 读取数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取单个SDE图层
            //GetSingleSDELayer();

            //获得所有SDE图层
            GetAllSDELayer();

        }
        
        /// <summary>
        /// 通过工作空间获取要素类
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private List<string> GetFeatureClassByWorkspace(IWorkspace workspace)
        {
            //定义个string类型的泛型类，用来存放要素类
            List<string> listFeatureClass = new List<string>();
            try
            {
                //将工作空间强转成要素工作空间
                IFeatureWorkspace pFeatureWorkspace = workspace as IFeatureWorkspace;
                //通过工作空间的get_Datasets方法获得所有数据集并强转成枚举数据集并存放在枚举数据集中
                IEnumDataset pEnumDataset = workspace.get_Datasets(esriDatasetType.esriDTAny) as IEnumDataset;
                //调用枚举数据集中的Next()方法指向单条数据集(第一条)
                IDataset pDataset = pEnumDataset.Next();
                //当数据集不为空时，遍历工作空间下的要素类或要素集
                while (pDataset != null)
                {
                    //判断数据集的类型是否和esri中的要素类类型相同
                    if (pDataset.Type == esriDatasetType.esriDTFeatureClass)
                    {
                        //数据集的名字加入到泛型要素类中
                        listFeatureClass.Add(pDataset.Name);
                    }

                    //判断数据集的类型是否和esri中的要素数据集相同
                    else if (pDataset.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        //将该要素集的子集赋值给枚举要素类
                        IEnumDataset pSubEnumDataset = pDataset.Subsets;
                        //调用枚举数据集中的Next()方法指向下一个单条数据集(为子集)
                        IDataset pSubDataset = pSubEnumDataset.Next();
                        //当数据集不为空时
                        while (pSubDataset != null)
                        {
                            //数据集的名字加入到泛型要素类中
                            listFeatureClass.Add(pSubDataset.Name);
                            //调用枚举数据集中的Next()方法指向下一个单条数据集
                            pSubDataset = pSubEnumDataset.Next();
                        }
                    }
                    //调用枚举数据集中的Next()方法指向单条数据集
                    pDataset = pEnumDataset.Next();
                }
            }
            catch (Exception ex)
            {
                return null;//出错返回null
            }
            //返回listFeatureClass泛型类
            return listFeatureClass;
        }

        /// <summary>
        /// 获得所有SDE图层
        /// </summary>
        private void GetAllSDELayer()
        {
            //SDE直接连接函数
            IPropertySet propset = SDEDirectConnection();

            //定义一个工作空间, 并实例化为SDE的工作空间类
            IWorkspaceFactory workspaceFactroy = new SdeWorkspaceFactoryClass();
            //打开SDE工作空间
            IWorkspace workspace = workspaceFactroy.Open(propset, 0);
            //通过工作空间获取要素类,并存放在List<string>的泛型类中
            List<string> listFeatureClass = GetFeatureClassByWorkspace(workspace);
            //定义个变量i，用来获取图层
            int i = 0;
            //使用foreach循环来遍历listFeatureClass
            foreach (var item in listFeatureClass)
            {
                //将工作空间强转成要素工作空间
                IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
                //通过要素空间打开要素类并存放在要素类中
                IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(item);
                //新建一个要素图层
                IFeatureLayer featureLayer = new FeatureLayer();
                //将要素类存放在刚定义好的要素图层的要素类中
                featureLayer.FeatureClass = featureClass;
                //设置要素图层名字
                featureLayer.Name = featureClass.AliasName;
                //axMapControl1增加图层
                axMapControl1.AddLayer(featureLayer, i);
                //自增i
                i++;
            }
            //刷新axMapControl1
            axMapControl1.Refresh();
        }

        /// <summary>
        /// 获取单个要素
        /// </summary>
        private void GetSingleSDELayer()
        {
            //SDE直接连接函数
            IPropertySet propset = SDEDirectConnection();

            //定义一个工作空间, 并实例化为SDE的工作空间类
            IWorkspaceFactory workspaceFactroy = new SdeWorkspaceFactoryClass();
            //打开SDE工作空间，并强转成要素工作空间
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactroy.Open(propset, 0);
            //通过要素空间打开要素类“sde.DBO.河流”并存放在要素类中
            IFeatureClass featureClass = featureWorkspace.OpenFeatureClass("sde.DBO.河流");
            //新建一个要素图层
            IFeatureLayer featureLayer = new FeatureLayerClass();
            //将要素类存放在刚定义好的要素图层的要素类中
            featureLayer.FeatureClass = featureClass;
            //设置要素图层名字
            featureLayer.Name = featureClass.AliasName;
            //axMapControl1增加图层
            axMapControl1.AddLayer(featureLayer);
            //刷新axMapControl1
            axMapControl1.Refresh();
        }

        /// <summary>
        /// SDE直接连接函数
        /// </summary>
        /// <returns></returns>
        private static IPropertySet SDEDirectConnection()
        {
            //定义一个数据库连接属性
            IPropertySet propset = new PropertySetClass();
            //采用SDE连接
            //设置数据库服务器名, 服务器所在的名称(即实例)
            propset.SetProperty("SERVER", @"USER-20190915QG\SQLEXPRESS");
            //设置SDE的端口,这是安装时指定的
            propset.SetProperty("INSTANCE", @"sde:sqlserver:USER-20190915QG\SQLEXPRESS");
            //SDE的用户名
            propset.SetProperty("USER", "sa");
            //SDE的密码
            propset.SetProperty("PASSWORD", "root");
            //设置数据库的名字，只有SQL Server Informix数据库才需要设置
            propset.SetProperty("DATABASE", "sde");
            //SDE的版本, 这里为默认版本
            propset.SetProperty("VERSION", "sde.Default");
            return propset;
        }
    }
}

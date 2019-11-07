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

        private void 读取数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSDELayer();
        }

        private void AddSDELayer()
        {
            //定义一个数据库连接属性
            IPropertySet propset = new PropertySetClass();
            //采用SDE连接
            //社渚数据库服务器名, 服务器所在的名称(即实例)IP地址？？
            propset.SetProperty("SERVER", @"USER-20190915QG\SQLEXPRESS");
            //设置SDE的端口,这是安装时指定的, 默认安装时"port:5151"??
            propset.SetProperty("INSTANCE", @"sde:sqlserver:USER-20190915QG\SQLEXPRESS");
            //SDE的用户名
            propset.SetProperty("USER", "sa");
            //密码
            propset.SetProperty("PASSWORD", "root");
            //设置数据库的名字，只有SQL Server Informix数据库才需要设置
            propset.SetProperty("DATABASE", "sde");
            //SDE的版本, 这里为默认版本
            propset.SetProperty("VERSION", "sde.Default");

            //定义一个工作空间, 并实例化为SDEd工作空间
            IWorkspaceFactory workspaceFactroy = new SdeWorkspaceFactoryClass();
            //打开SDE工作空间，并强转为要素工作空间
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactroy.Open(propset, 0);

            IFeatureClass featureClass = featureWorkspace.OpenFeatureClass("sde.DBO.river");
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            axMapControl1.AddLayer(featureLayer);
            axMapControl1.Refresh();
        }
    }
}

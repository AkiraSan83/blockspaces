/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 * 
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace SQLite.Designer
{
  using System;
  using Microsoft.VisualStudio.Shell;
  using System.Runtime.InteropServices;
  using System.ComponentModel.Design;
  using Microsoft.VisualStudio.Shell.Interop;

  /// <summary>
  /// Ideally we'd be a package provider, but the VS Express Editions don't support us, so this class
  /// exists so that in the future we can perhaps work with the Express Editions.
  /// </summary>
  [Guid("DCBE6C8D-0E57-4099-A183-98FF74C64D9C")]
  internal sealed class SQLitePackage : Package
  {
    public SQLitePackage()
    {
    }

    protected override void Initialize()
    {
      IServiceContainer sc = (IServiceContainer)this;
      sc.AddService(typeof(SQLiteProviderObjectFactory), new ServiceCreatorCallback(CreateService), true);

      ToolboxInitialized += new EventHandler(SQLitePackage_ToolboxInitialized);
      ToolboxUpgraded += new EventHandler(SQLitePackage_ToolboxUpgraded);
      base.Initialize();
    }

    void SQLitePackage_ToolboxUpgraded(object sender, EventArgs e)
    {
      IVsToolbox vstbx = GetService(typeof(SVsToolbox)) as IVsToolbox;

      vstbx.RemoveTab("SQLite");

      SQLitePackage_ToolboxInitialized(sender, e);
    }

    void SQLitePackage_ToolboxInitialized(object sender, EventArgs e)
    {
      ParseToolboxResource(new System.IO.StringReader(VSPackage.ToolboxItems), null);
    }

    private object CreateService(IServiceContainer container, Type serviceType)
    {
      if (serviceType == typeof(SQLiteProviderObjectFactory))
        return new SQLiteProviderObjectFactory();

      return null;
    }
  }
}

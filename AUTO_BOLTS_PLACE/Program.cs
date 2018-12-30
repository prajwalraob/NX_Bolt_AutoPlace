using System;
using System.IO;
using System.Collections.Generic;

using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;

public class Program
{
    // class members
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;
	double pi = 3.1428;

    //------------------------------------------------------------------------------
    // Constructor
    //------------------------------------------------------------------------------
    public Program()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
            isDisposeCalled = false;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            // UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
        }
    }

    //------------------------------------------------------------------------------
    //  Explicit Activation
    //      This entry point is used to activate the application explicitly
    //------------------------------------------------------------------------------
    public static int Main(string[] args)
    {
        int retValue = 0;
        try
        {
            theProgram = new Program();

            //theSession.Parts.CloseAll(BasePart.CloseModified.DontCloseModified, PCL);
        }
        catch (NXOpen.NXException ex)
        {


        }
        theProgram.Dispose();
        return retValue;
    }

    //------------------------------------------------------------------------------
    //  NX Startup
    //      This entry point activates the application at NX startup

    //Will work when complete path of the dll is provided to Environment Variable 
    //USER_STARTUP or USER_DEFAULT

    //OR

    //Will also work if dll is at folder named "startup" under any folder listed in the 
    //text file pointed to by the environment variable UGII_CUSTOM_DIRECTORY_FILE.
    //------------------------------------------------------------------------------
    public static int Startup()
    {
        double pi = 3.1428;
        int retValue = 0;
        try
        {
			/*
                Before starting implementation, check how to start the assembly with full part load option;
            currently the load options only enabled for partial loading. See how to set it permanently or
            for session start with API.

                First part of the trial task is to load and change the parameter of the bolt.
            For test change clamping distance. (27/12)
            */
            theProgram = new Program();        
			System.IO.StreamWriter SW = new System.IO.StreamWriter("D:\\GIT\\AUTO_BOLTS_PLACE\\NXDebugLog.log");

            theProgram.LoadAssmbly();
 
            theUfSession.Part.CloseAll();
            SW.Close();
            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            System.IO.StreamWriter SW = new System.IO.StreamWriter("D:\\GIT\\AUTO_BOLTS_PLACE\\NXErrorLog.log");
            SW.WriteLine(ex.Message.ToString());
            SW.Close();

            theUfSession.Part.CloseAll();

            theSession.ListingWindow.Open();
            theSession.ListingWindow.WriteLine("Error occurred");
            theSession.ListingWindow.WriteLine(ex.Message.ToString());
        }
        //theProgram.Dispose();
        return retValue;
    }

    //------------------------------------------------------------------------------
    // Following method disposes all the class members
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        try
        {
            if (isDisposeCalled == false)
            {
                //TODO: Add your application code here 
            }
            isDisposeCalled = true;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
    }

    public static int GetUnloadOption(string arg)
    {
        //Unloads the image explicitly, via an unload dialog
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);

        //Unloads the image immediately after execution within NX
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

        //Unloads the image when the NX session terminates
        return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }
	
	    private Dictionary<string, string> Dirs(string SourcePath, string Target)
    {
        string specification = "M20x35";
        Dictionary<string, string> NewDict = new Dictionary<string, string>();

        foreach (string dirPath in Directory.GetFiles(SourcePath, "*", SearchOption.AllDirectories))
        {
            string temp = System.IO.Path.GetFileNameWithoutExtension(dirPath);
            string ext = System.IO.Path.GetExtension(dirPath);
            temp = temp + "_" + specification + ext;

            string boltpath = "BOLT_" + specification;

            string newdir = System.IO.Path.Combine(Target, boltpath);
            string final = System.IO.Path.Combine(newdir, temp);

            if (!(Directory.Exists(newdir)))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(SourcePath, boltpath));
            }

            File.Copy(dirPath, final, true);

            string partanme = Path.GetFileNameWithoutExtension(final);

            try
            {
                if (partanme.Contains("BOLT"))
                {
                    NewDict.Add("BOLT", final);
                }

                if (partanme.Contains("NUT"))
                {
                    NewDict.Add("NUT", final);
                }

                if (partanme.Contains("ASSEMBLY"))
                {
                    NewDict.Add("ASM", final);
                }

                if (partanme.Contains("WASHER"))
                {
                    NewDict.Add("WSH", final);
                }
            }
            catch (Exception E)
            {
            }
        }

        return NewDict;
    }

    private void LoadAssmbly()
    {
        string PartPath = "D:\\ACCEPTED_NX_MODELS\\NUT_BOLT\\MASTERS";
        string TargetFolder = "D:\\ACCEPTED_NX_MODELS\\PIPES";

        Dictionary<string, string> NS = theProgram.Dirs(PartPath, TargetFolder);

        string Asms = NS["ASM"];

        theSession.Parts.LoadOptions.UsePartialLoading = false;

        //https://community.plm.automation.siemens.com/t5/NX-Programming-Customization-Forum/How-to-fully-load-a-part-using-NXOpen/td-p/338330

        PartLoadStatus PLD;
        BasePart Asm = theSession.Parts.OpenDisplay(Asms, out PLD);

        PlaceDefaultPos(Asm, NS["NUT"]);
        PlaceDefaultPos(Asm, NS["BOLT"]);
        PlaceDefaultPos(Asm, NS["WSH"]);
        PlaceDefaultPos(Asm, NS["WSH"]);

        PartSaveStatus PS;
        bool Partsmod;
        theSession.Parts.SaveAll(out Partsmod, out PS);

        Component Comp = Asm.ComponentAssembly.RootComponent;

        foreach (Component C in Comp.GetChildren())
        {
            BasePart T = C.OwningPart;
            T.LoadFully();

        }
    }

    void PlaceDefaultPos(BasePart baseprt ,string partpath)
    {

        Point3d basePoint = new NXOpen.Point3d(0.0, 0.0, 0.0);
        Matrix3x3 orientation = new NXOpen.Matrix3x3();

        orientation.Xx = 0; orientation.Xy = 0.0; orientation.Xz = -1;
        orientation.Yx = 0.0; orientation.Yy = 1; orientation.Yz = 0;
        orientation.Zx = 1; orientation.Zy = 0; orientation.Zz = 0;

        string partname = Path.GetFileNameWithoutExtension(partpath);

        PartLoadStatus partLoadStatus1;
        Component component1;
        component1 = baseprt.ComponentAssembly.AddComponent(partpath, "MODEL", partname, basePoint, orientation, -1, out partLoadStatus1, true);

    }

}


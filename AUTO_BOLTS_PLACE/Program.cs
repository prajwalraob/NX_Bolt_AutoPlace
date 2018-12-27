using System;
using NXOpen;
using NXOpen.UF;

public class Program
{
    // class members
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;

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
        double pi = 3.1428;
        try
        {
            theProgram = new Program();
            System.IO.StreamWriter SW = new System.IO.StreamWriter("D:\\ACCEPTED_NX_MODELS\\NXDebugLog.log");
            string PartPath = "D:\\ACCEPTED_NX_MODELS\\PIPES\\PIPE_ASM.prt";

            PartLoadStatus PLD;
            //Part prt = theSession.Parts.OpenDisplay(PartPath, out PLD);
            Part wp = theSession.Parts.Work;
            Part prt = theSession.Parts.Display;

            NXOpen.Assemblies.ComponentAssembly Asm = prt.ComponentAssembly;
            NXOpen.Assemblies.Component[] Col = Asm.RootComponent.GetChildren();

            string addPart = "D:\\ACCEPTED_NX_MODELS\\PIPES\\BOLT_ASM.prt";
            Point3d point;
            point.X = 0; point.Y = -8; point.Z = 85 / 2;
            Matrix3x3 matrix = default(Matrix3x3);
            matrix.Xx = 1; matrix.Xy = 0; matrix.Xz = 0;
            matrix.Yx = 0; matrix.Yy = 0; matrix.Yz = 1;
            matrix.Zx = 0; matrix.Zy = -1; matrix.Zz = pi / 2;
            Asm.AddComponent(addPart, "MDL", "BOLT", point, matrix, 254, out PLD);

            SW.Close();
            prt.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
            PartCloseResponses PCL = default(PartCloseResponses);
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
            theProgram = new Program();
            System.IO.StreamWriter SW = new System.IO.StreamWriter("D:\\ACCEPTED_NX_MODELS\\NXDebugLog.log");
            string PartPath = "D:\\ACCEPTED_NX_MODELS\\PIPES\\PIPE_ASM.prt";

            PartLoadStatus PLD;
            Part prt = theSession.Parts.Open(PartPath, out PLD);
            Part wp = theSession.Parts.Work;
            Part dp = theSession.Parts.Display;

            NXOpen.Assemblies.ComponentAssembly Asm = prt.ComponentAssembly;
            NXOpen.Assemblies.Component[] Col = Asm.RootComponent.GetChildren();

            string addPart = "D:\\ACCEPTED_NX_MODELS\\PIPES\\BOLT_ASM.prt";
            Point3d point;
            point.X = 0; point.Y = 0; point.Z = 0;
            Matrix3x3 matrix = default(Matrix3x3);
            matrix.Xx = pi / 4; matrix.Xy = 1; matrix.Xz = 0;
            matrix.Yx = -1; matrix.Yy = 0; matrix.Yz = 0;
            matrix.Zx = 0; matrix.Zy = 0; matrix.Zz = 0;
            Asm.AddComponent(addPart, "MDL", "BOLT", point, matrix, 254, out PLD);

            SW.Close();
            prt.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.True);
            PartCloseResponses PCL = default(PartCloseResponses);
            //theSession.Parts.CloseAll(BasePart.CloseModified.DontCloseModified, PCL);

        }
        catch (NXOpen.NXException ex)
        {
            System.IO.StreamWriter SW = new System.IO.StreamWriter("D:\\ACCEPTED_NX_MODELS\\NXErrorLog.log");
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

}


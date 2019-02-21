using System;
using System.IO;
using System.Collections.Generic;

using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;

public class Program
{
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;
	double pi = 3.1428;

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
            // UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
        }
    }

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
			//System.IO.StreamWriter SW = new System.IO.StreamWriter("D:\\GIT\\AUTO_BOLTS_PLACE\\NXDebugLog.log");

            AssemblyProcessor Processor = new AssemblyProcessor(ref theSession);
            Processor.LoadAssmbly();
            Processor.ConstraintCreate();
 
            theUfSession.Part.CloseAll();
            //SW.Close();
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
        return retValue;
    }

    public void Dispose()
    {
        try
        {
            if (isDisposeCalled == false)
            {

            }
            isDisposeCalled = true;
        }
        catch (NXOpen.NXException ex)
        {

        }
    }

    public static int GetUnloadOption(string arg)
    {
        return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }
}


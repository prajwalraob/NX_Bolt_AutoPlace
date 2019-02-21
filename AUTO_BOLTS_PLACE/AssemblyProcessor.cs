using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using NXOpen;
using NXOpen.Assemblies;

class AssemblyProcessor
{
    private Session newSession;
    private Program newProgram;
    private Dictionary<string, string> NS;
    private string PartPath = @"D:\ACCEPTED_NX_MODELS\NUT_BOLT\MASTERS";
    private string TargetFolder = @"D:\ACCEPTED_NX_MODELS\PIPES";
    private BasePart Assembly;

    public AssemblyProcessor(ref Session AppSession)
    {
        newProgram = new Program();
        newSession = AppSession;
    }

    public void LoadAssmbly()
    {
       this.Dirs();

        string Asms = NS["ASM"];

        newSession.Parts.LoadOptions.UsePartialLoading = false;

        //https://community.plm.automation.siemens.com/t5/NX-Programming-Customization-Forum/How-to-fully-load-a-part-using-NXOpen/td-p/338330

        PartLoadStatus PLD;
        BasePart Asm = newSession.Parts.OpenDisplay(Asms, out PLD);

        PlaceDefaultPos(Asm, NS["NUT"]);
        PlaceDefaultPos(Asm, NS["BOLT"]);
        PlaceDefaultPos(Asm, NS["WSH"]);
        PlaceDefaultPos(Asm, NS["WSH"]);

        Assembly = Asm;

        PartSaveStatus PS;
        bool Partsmod;
        newSession.Parts.SaveAll(out Partsmod, out PS);

        //Component Comp = Asm.ComponentAssembly.RootComponent;

        //foreach (Component C in Comp.GetChildren())
        //{
        //    BasePart T = C.OwningPart;
        //    T.LoadFully();

        //}
    }

    void PlaceDefaultPos(BasePart baseprt, string partpath)
    {

        Point3d basePoint = new NXOpen.Point3d(0.0, 0.0, 0.0);
        Matrix3x3 orientation = new NXOpen.Matrix3x3();

        orientation.Xx = 0; orientation.Xy = 0.0; orientation.Xz = -1;
        orientation.Yx = 0.0; orientation.Yy = 1; orientation.Yz = 0;
        orientation.Zx = 1; orientation.Zy = 0; orientation.Zz = 0;

        string partname = Path.GetFileNameWithoutExtension(partpath);

        PartLoadStatus partLoadStatus1;
        Component component1;
        component1 = baseprt.ComponentAssembly.AddComponent(partpath, "Entire Part", partname, basePoint, orientation, -1, out partLoadStatus1, true);

    }

    private void Dirs()
    {
        string specification = "M20x35";
        NS = new Dictionary<string, string>();

        foreach (string dirPath in Directory.GetFiles(PartPath, "*", SearchOption.AllDirectories))
        {
            string temp = System.IO.Path.GetFileNameWithoutExtension(dirPath);
            string ext = System.IO.Path.GetExtension(dirPath);
            temp = temp + "_" + specification + ext;

            string boltpath = "BOLT_" + specification;

            string newdir = System.IO.Path.Combine(TargetFolder, boltpath);
            string final = System.IO.Path.Combine(newdir, temp);

            if (!(Directory.Exists(newdir)))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(PartPath, boltpath));
            }

            File.Copy(dirPath, final, true);

            string partanme = Path.GetFileNameWithoutExtension(final);

            try
            {
                if (partanme.Contains("BOLT"))
                {
                    NS.Add("BOLT", final);
                }

                if (partanme.Contains("NUT"))
                {
                    NS.Add("NUT", final);
                }

                if (partanme.Contains("ASSEMBLY"))
                {
                    NS.Add("ASM", final);
                }

                if (partanme.Contains("WASHER"))
                {
                    NS.Add("WSH", final);
                }
            }
            catch (Exception E)
            {
            }
        }
    }

    public void ConstraintCreate()
    {
        System.IO.StreamWriter SW = new System.IO.StreamWriter("D:\\GIT\\AUTO_BOLTS_PLACE\\NXDebugLog.log");
        Component Comp = Assembly.ComponentAssembly.RootComponent;

        //now what does NX tags do? Need to know.
        //how to make use of tags to obtain the parts
        //https://community.plm.automation.siemens.com/t5/NX-Programming-Customization-Forum/Creating-Assesbly-Constraint-using-NXopen/td-p/476978

        NXOpen.Layer.StateInfo[] stateArray1 = new NXOpen.Layer.StateInfo[1];
        stateArray1[0] = new NXOpen.Layer.StateInfo(32, NXOpen.Layer.State.Selectable);
        Assembly.Layers.ChangeStates(stateArray1, false);

        foreach (Component C in Comp.GetChildren())
        {
            string name = C.Name;

            Part owningpart = (NXOpen.Part)C.OwningPart;
            owningpart.LoadFully();

            DatumCollection dc = owningpart.Datums;

            try
            {
                foreach (DatumPlane d in dc)
                {
                    string nam2 = d.Feature.Name;
                    string nam3 = d.Layer.ToString();
                    SW.WriteLine(nam3);
                    //string nam3 = d.
                }
            }
            catch (Exception E3)
            { }
                       
        }

        SW.Close();
    }
}


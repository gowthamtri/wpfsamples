using System;
using RobotOM;

namespace RobotAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new RobotApplication();

            app.Project.Open(@"C:\Users\GowthamanS\Desktop\temp\Revit Files\SlabDesign\Structure.rtd");

            app.Project.CalcEngine.Calculate();

            var collection = app.Project.Structure.FiniteElems.GetAll();
            var loadCases = app.Project.Structure.Cases.GetAll();

            for (var i = 0; i < collection.Count; i++)
            {
                var fe = collection.Get(i) as IRobotFiniteElement;
                if (fe != null)
                {
                    var number = fe.Number;
                    Console.WriteLine($"{number} - {fe.FeType}");
                    for (var j = 0; j < loadCases.Count; j++)
                    {
                        try
                        {
                            var loadCase = loadCases.Get(j);// as IRobotCaseCombination;
                            var loadCaseId = loadCase.Number;

                            var param = new RobotFeResultParams();
                            param.Element = number;
                            param.Case = loadCaseId;

                            param.Layer = IRobotFeLayerType.I_FLT_ARBITRARY;
                            param.LayerArbitraryValue = 0.25;

                            var princi = app.Project.Structure.Results.FiniteElems.Principal(param);
                            var res_mom = princi.M1_2;
                            var res_for = princi.NAL;
                            var res_str = princi.S1;
                            var res_dis = princi.UGX;

                            Console.WriteLine($"{res_mom}_{res_for}_{res_str}_{res_dis}");
                        }
                        catch
                        {
                            Console.WriteLine("Error Loadcase");
                        }
                    }
                }
            }

            app.Project.Close();

            app = null;

            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RobotOM;

namespace SamplePlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //GetFEMeshQuery(out var panelIds, out var coords, out var indices);
                FromTutorial();
                DumpMesh();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void DumpMesh()
        {
            var RobApp = new RobotApplication();

            var t = RobApp.Project.ViewMngr.CreateTable(IRobotTableType.I_TT_FINITE_ELEMENTS, IRobotTableDataType.I_TDT_FE);
            var tf = RobApp.Project.ViewMngr.GetTable(1);

            var FName = tf.Window.Caption;

            t.Select(IRobotSelectionType.I_ST_PANEL, "all");
            t.Select(IRobotSelectionType.I_ST_FINITE_ELEMENT, "all");
            tf.Get(2).Window.Activate();

            t = tf.Get(2);
            tf.Current = 2;
            var tabname = tf.GetName(2);
            tabname = tabname.Replace(" ", "_");

            t.Printable.SaveToFile(@"C:\Users\GowthamanS\Desktop\Fe.txt", IRobotOutputFileFormat.I_OFF_TEXT);


            var nodes = RobApp.Project.Structure.Nodes.GetAll();
            var stringBuilder = new StringBuilder();
            foreach (var i in Enumerable.Range(1, nodes.Count))
            {
                var node = nodes.Get(i) as RobotNode;
                stringBuilder.AppendLine($"{node.Number}, {node.X}, {node.Y}, {node.Z}");
            }

            File.WriteAllText(@"C:\Users\GowthamanS\Desktop\FeNodes.txt", stringBuilder.ToString());
        }

        /// <summary>
        /// Gets the FE meshes from a Robot model using the fast query method
        /// </summary>
        /// <param name="panel_ids"></param>
        /// <param name="coords"></param>
        /// <param name="vertex_indices"></param>
        /// <param name="str_nodes"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool GetFEMeshQuery(out int[] panel_ids, out double[][] coords, out Dictionary<int, int[]> vertex_indices, string filePath = "LiveLink")
        {
            RobotApplication robot = null;
            if (filePath == "LiveLink") robot = new RobotApplication();

            //First call getnodesquery to get node points
            double[][] nodeCoords = null;

            //Dictionary<int, BHoM.Structural.Node> _str_nodes = new Dictionary<int, BHoM.Structural.Node>();
            //RobotToolkit.Node.GetNodesQuery(project, filePath);
            //Dictionary<int, int> _nodeIds = new Dictionary<int, int>();
            //for (int i = 0; i < _str_nodes.Count; i++)
            //{
            //    _nodeIds.Add(_str_nodes.ElementAt(i).Value.Number, i);
            //}

            RobotResultQueryParams result_params = (RobotResultQueryParams)robot.Kernel.CmpntFactory.Create(IRobotComponentType.I_CT_RESULT_QUERY_PARAMS);
            RobotStructure rstructure = robot.Project.Structure;
            RobotSelection FE_sel = rstructure.Selections.CreateFull(IRobotObjectType.I_OT_FINITE_ELEMENT);
            IRobotResultQueryReturnType query_return = IRobotResultQueryReturnType.I_RQRT_MORE_AVAILABLE;
            RobotSelection cas_sel = rstructure.Selections.Create(IRobotObjectType.I_OT_CASE);
            try { cas_sel.FromText(robot.Project.Structure.Cases.Get(1).Number.ToString()); } catch { }

            if (cas_sel.Count > 0) result_params.Selection.Set(IRobotObjectType.I_OT_CASE, cas_sel);

            //result_params.Selection.Set(IRobotObjectType.I_OT_NODE, FE_sel);

            result_params.SetParam(IRobotResultParamType.I_RPT_MULTI_THREADS, true);
            result_params.SetParam(IRobotResultParamType.I_RPT_THREAD_COUNT, 4);
            result_params.SetParam(IRobotResultParamType.I_RPT_SMOOTHING, IRobotFeResultSmoothing.I_FRS_NO_SMOOTHING);
            result_params.SetParam(IRobotResultParamType.I_RPT_DIR_X_DEFTYPE, IRobotObjLocalXDirDefinitionType.I_OLXDDT_CARTESIAN);
            result_params.SetParam(IRobotResultParamType.I_RPT_DIR_X, new double[] { 1, 0, 0 });

            result_params.SetParam(IRobotResultParamType.I_RPT_NODE, 1);
            result_params.SetParam(IRobotResultParamType.I_RPT_PANEL, 1);
            result_params.SetParam(IRobotResultParamType.I_RPT_ELEMENT, 1);
            result_params.SetParam(IRobotResultParamType.I_RPT_RESULT_POINT_COORDINATES, 1);

            result_params.ResultIds.SetSize(2);
            result_params.ResultIds.Set(1, (int)IRobotFeResultType.I_FRT_DETAILED_MXX);
            result_params.ResultIds.Set(2, (int)IRobotFeResultType.I_FRT_DETAILED_MYY);

            RobotResultRowSet row_set = new RobotResultRowSet();
            bool ok = false;
            RobotResultRow result_row = default(RobotResultRow);

            List<int> _panel_ids = new List<int>();
            Dictionary<int, int[]> _vertex_indices = new Dictionary<int, int[]>();
            int kounta = 0;

            while (!(query_return == IRobotResultQueryReturnType.I_RQRT_DONE))
            {
                query_return = rstructure.Results.Query(result_params, row_set);
                ok = row_set.MoveFirst();
                while (ok)
                {
                    //int panel_num = (int)row_set.CurrentRow.GetValue(1252);
                    //_panel_ids.Add(panel_num);

                    int nodeId = (int)row_set.CurrentRow.GetParam(IRobotResultParamType.I_RPT_NODE);
                    //int panelId = (int)row_set.CurrentRow.GetParam(IRobotResultParamType.I_RPT_PANEL);
                    int elementId = (int)row_set.CurrentRow.GetParam(IRobotResultParamType.I_RPT_ELEMENT);

                    //int number_of_indices = (row_set.CurrentRow.IsAvailable(567)) ? 4 : 3;
                    //int[] temp_indices = new int[number_of_indices];
                    //for (int i = 0; i < number_of_indices; i++)
                    //{
                    //    temp_indices[i] = (int)row_set.CurrentRow.GetValue(564 + i);
                    //}

                    var resultIds = row_set.ResultIds;

                    var xxxx = row_set.CurrentRow.GetParam(IRobotResultParamType.I_RPT_RESULT_POINT_COORDINATES);



                    //_vertex_indices.Add(kounta, temp_indices);
                    //kounta++;
                    ok = row_set.MoveNext();
                }
                row_set.Clear();
            }
            result_params.Reset();

            panel_ids = _panel_ids.ToArray();
            vertex_indices = _vertex_indices;
            coords = nodeCoords;
            //str_nodes = _str_nodes;
            return true;
        }

        private static void FromTutorial()
        {
            var robApp = new RobotApplication();

            if (robApp.Visible == 0)
            {
                robApp.Interactive = 1;
                robApp.Visible = 1;
            }
            robApp.Project.New(IRobotProjectType.I_PT_SHELL);
            var projectPrefs = robApp.Project.Preferences;
            projectPrefs.SetActiveCode(IRobotCodeType.I_CT_RC_THEORETICAL_REINF, "BAEL 91");
            var meshParams = projectPrefs.MeshParams;
            meshParams.SurfaceParams.Method.Method = IRobotMeshMethodType.I_MMT_DELAUNAY;
            meshParams.SurfaceParams.Generation.Type = IRobotMeshGenerationType.I_MGT_ELEMENT_SIZE;
            meshParams.SurfaceParams.Generation.ElementSize = 0.5;
            meshParams.SurfaceParams.Delaunay.Type = IRobotMeshDelaunayType.I_MDT_DELAUNAY;

            IRobotStructure str = robApp.Project.Structure;
            str.Nodes.Create(1, 0, 0, 0);
            str.Nodes.Create(2, 3, 0, 0);
            str.Nodes.Create(3, 3, 3, 0);
            str.Nodes.Create(4, 0, 3, 0);
            str.Nodes.Create(5, 0, 0, 4);

            str.Nodes.Create(6, 3, 0, 4);
            str.Nodes.Create(7, 3, 3, 4);
            str.Nodes.Create(8, 0, 3, 4);

            str.Bars.Create(1, 1, 5);
            str.Bars.Create(2, 2, 6);
            str.Bars.Create(3, 3, 7);
            str.Bars.Create(4, 4, 8);
            str.Bars.Create(5, 5, 6);
            str.Bars.Create(6, 7, 8);

            var labels = str.Labels;
            var columnSectionName = "Rect. Column 30*30";

            var label = labels.Create(IRobotLabelType.I_LT_BAR_SECTION, columnSectionName);
            var section = (RobotBarSectionData)label.Data;
            section.ShapeType = IRobotBarSectionShapeType.I_BSST_CONCR_COL_R;

            var concrete = (RobotBarSectionConcreteData)section.Concrete;
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_COL_B, 0.3);
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_COL_H, 0.3);
            section.CalcNonstdGeometry();
            labels.Store(label);
            var selectionBars = str.Selections.Get(IRobotObjectType.I_OT_BAR);
            selectionBars.FromText("1 2 3 4");
            str.Bars.SetLabel(selectionBars, IRobotLabelType.I_LT_BAR_SECTION, columnSectionName);
            var steelSections = projectPrefs.SectionsActive;
            if (steelSections.Add("RCAT") == 1)
                Console.WriteLine("Steel section base RCAT not found...");
            selectionBars.FromText("5 6");
            label = labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "HEA 340");
            str.Labels.Store(label);
            str.Bars.SetLabel(selectionBars, IRobotLabelType.I_LT_BAR_SECTION, "HEA 340");
            var materialName = "Concrete 30";
            label = labels.Create(IRobotLabelType.I_LT_MATERIAL, materialName);
            var material = (RobotMaterialData)label.Data;
            material.Type = IRobotMaterialType.I_MT_CONCRETE;
            material.E = 30000000000; // Young
            material.NU = 1 / (double)6; // Poisson

            material.RO = 25000; // Unit weight
            material.Kirchoff = material.E / (2 * (1 + material.NU));
            str.Labels.Store(label);
            var points = (RobotPointsArray)robApp.CmpntFactory.Create(IRobotComponentType.I_CT_POINTS_ARRAY);
            points.SetSize(5);
            points.Set(1, 0, 0, 4);
            points.Set(2, 3, 0, 4);
            points.Set(3, 3, 3, 4);
            points.Set(4, 0, 3, 4);
            points.Set(5, 0, 0, 4);
            var slabSectionName = "Slab 30";
            label = labels.Create(IRobotLabelType.I_LT_PANEL_THICKNESS, slabSectionName);
            var thickness = (RobotThicknessData)label.Data;
            thickness.MaterialName = materialName;
            thickness.ThicknessType = IRobotThicknessType.I_TT_HOMOGENEOUS;
            var thicknessData = (RobotThicknessHomoData)thickness.Data;
            thicknessData.ThickConst = 0.3;
            labels.Store(label);
            var objNumber = str.Objects.FreeNumber;
            str.Objects.CreateContour(objNumber, points);
            var slab = (RobotObjObject)str.Objects.Get(objNumber);
            slab.Main.Attribs.Meshed = 1;
            slab.SetLabel(IRobotLabelType.I_LT_PANEL_THICKNESS, slabSectionName);
            slab.Initialize();
            points.Set(1, 1.1, 1.1, 4);
            points.Set(2, 2.5, 1.1, 4);
            points.Set(3, 2.5, 2.5, 4);
            points.Set(4, 1.1, 2.5, 4);
            points.Set(5, 1.1, 1.1, 4);
            var holeNumber = str.Objects.FreeNumber;
            str.Objects.CreateContour(holeNumber, points);
            var hole = (RobotObjObject)str.Objects.Get(holeNumber);
            hole.Main.Attribs.Meshed = 0;
            hole.Initialize();
            var footName = "Foot";
            label = labels.Create(IRobotLabelType.I_LT_SUPPORT, footName);
            var footData = (RobotNodeSupportData)label.Data;
            footData.UX = 1;
            footData.UY = 1;
            footData.UZ = 1;
            footData.KZ = 80000000;
            footData.RX = 1;

            footData.RY = 1;
            footData.RZ = 1;
            labels.Store(label);
            var selectionNodes = str.Selections.Get(IRobotObjectType.I_OT_NODE);
            selectionNodes.FromText("1 2 3 4");
            str.Nodes.SetLabel(selectionNodes, IRobotLabelType.I_LT_SUPPORT, footName);
            //self weight on entire structure
            var caseSw = str.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            caseSw.Records.New(IRobotLoadRecordType.I_LRT_DEAD);
            var loadRecord = (RobotLoadRecord)caseSw.Records.Get(1);
            loadRecord.SetValue(System.Convert.ToInt16(IRobotDeadRecordValues.I_DRV_Z), -1);
            loadRecord.SetValue(System.Convert.ToInt16(IRobotDeadRecordValues.I_DRV_ENTIRE_STRUCTURE), 0);
            //contour live load on the slab
            var caseLive = str.Cases.CreateSimple(2, "Live", IRobotCaseNature.I_CN_EXPLOATATION, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            var uniform = caseLive.Records.New(IRobotLoadRecordType.I_LRT_UNIFORM);
            loadRecord = (RobotLoadRecord)caseLive.Records.Get(uniform);
            loadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PX), 0);
            loadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PY), 0);
            loadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PZ), -10000);
            //apply created load to the slab
            loadRecord.Objects.FromText(System.Convert.ToString(objNumber));
            //linear wind load on the beam
            var caseWind = str.Cases.CreateSimple(3, "Wind", IRobotCaseNature.I_CN_WIND, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            uniform = caseWind.Records.New(IRobotLoadRecordType.I_LRT_BAR_UNIFORM);
            loadRecord = (RobotLoadRecord)caseWind.Records.Get(uniform);
            loadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PX), 0);
            loadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PY), 1000);

            loadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PZ), 0);
            //apply created load to the beam
            loadRecord.Objects.FromText("5");
            var calcEngine = robApp.Project.CalcEngine;
            calcEngine.GenerationParams.GenerateNodes_BarsAndFiniteElems = true;
            calcEngine.UseStatusWindow = true;

            if (calcEngine.Calculate() == 1)
                Console.WriteLine("Calculation Failed!");
            else
                Console.WriteLine("Done!");

            calcEngine = null;

            //CalcReinforcement(robApp, objNumber, labels, slab, str);
        }

        private static void CalcReinforcement(RobotApplication robApp, int objNumber, RobotLabelServer labels, RobotObjObject slab, IRobotStructure str)
        {
            IRobotLabel label;
            var concrCalcEngine = robApp.Project.ConcrReinfEngine;
            var concrSlabRequiredReinfEngine = concrCalcEngine.SlabRequiredReinf;
            var slabRnfParams = concrSlabRequiredReinfEngine.Params;
            slabRnfParams.Method = IRobotReinforceCalcMethods.I_RCM_WOOD_ARMER;
            slabRnfParams.GloballyAvgDesginForces = false;
            slabRnfParams.ForcesReduction = false;
            slabRnfParams.DisplayErrors = false;
            slabRnfParams.CasesULS.FromText("1 2 3 4 5 6 7 8");
            var slabs = slabRnfParams.Panels;
            slabs.FromText(System.Convert.ToString(objNumber));
            var slabReinforcementName = "Slab X";
            label = labels.Create(IRobotLabelType.I_LT_PANEL_REINFORCEMENT, slabReinforcementName);
            labels.Store(label);
            slab.SetLabel(IRobotLabelType.I_LT_PANEL_REINFORCEMENT, slabReinforcementName);
            slab.Update();

            Console.WriteLine(!concrSlabRequiredReinfEngine.Calculate() ? "Calculation Failed!" : "Done!");
            //getting results My and Yz for beam (bar 5) with live load (case 2)
            var txt = "Bar 5, Live at 0.5 length:" + "\n\r" +
                      " My = " + str.Results.Bars.Forces.Value(5, 2, 0.5).MY / 1000 + " [kN*m]" + "\n\r" +
                      " Qz = " + -str.Results.Bars.Deflections.Value(5, 2, 0.5).UZ * 1000 + " [mm]" + "\n\r" +
                      " Fz1 = " + str.Results.Bars.Forces.Value(5, 2, 0).FZ / 1000 + " [kN]" + "\n\r" +
                      " Fz2 = " + str.Results.Bars.Forces.Value(5, 2, 1).FZ / 1000 + " [kN]" + "\n\r";
            //getting results Fx and Fy for column (bar 4) with wind load (case 3)
            txt += "Bar 4, Wind:" + "\n\r" +
                   " Fx = " + str.Results.Bars.Forces.Value(4, 3, 1).FX / 1000 + " [kN]" + "\n\r" +
                   " Fy = " + str.Results.Bars.Forces.Value(4, 3, 1).FY / 1000 + " [kN]" + "\n\r";
            //getting results Fx, Fy, Fz, Mx, My, Mz for foot (node 1) with self-weight (case 1)
            txt += "Node 1, Self-Weight:" + "\n\r" +
                   " Fx = " + str.Results.Nodes.Reactions.Value(1, 1).FX / 1000 + " [kN]" + "\n\r" +
                   " Fy = " + str.Results.Nodes.Reactions.Value(1, 1).FY / 1000 + " [kN]" + "\n\r" +
                   " Fz = " + str.Results.Nodes.Reactions.Value(1, 1).FZ / 1000 + " [kN]" + "\n\r" +
                   " Mx = " + str.Results.Nodes.Reactions.Value(1, 1).MX / 1000 + " [kN]" + "\n\r" +
                   " My = " + str.Results.Nodes.Reactions.Value(1, 1).MY / 1000 + " [kN]" + "\n\r" +
                   " Mz = " + str.Results.Nodes.Reactions.Value(1, 1).MZ / 1000 + " [kN]" + "\n\r";
            //getting results Ax+, Ax-, Ay+, Ay- for slab
            var selectionFe = str.Selections.Get(IRobotObjectType.I_OT_FINITE_ELEMENT);
            selectionFe.FromText(slab.FiniteElems);
            var objFEs = (RobotLabelCollection)str.FiniteElems.GetMany(selectionFe);
            double a;
            a = 0;
            double axP = 0;
            double axM = 0;
            double ayP = 0;
            double ayM = 0;
            var finiteElementNodes = new List<RobotFiniteElement>();
            for (var n = 1; n <= objFEs.Count; n++)
            {
                var fe = (RobotFiniteElement)objFEs.Get(n);
                finiteElementNodes.Add(fe);
                a = str.Results.FiniteElems.Reinforcement(slab.Number, fe.Number).AX_BOTTOM;
                if (a > axM)
                    axM = a;
                a = str.Results.FiniteElems.Reinforcement(slab.Number, fe.Number).AX_TOP;
                if (a > axP)
                    axP = a;

                a = str.Results.FiniteElems.Reinforcement(slab.Number, fe.Number).AY_BOTTOM;
                if (a > ayM)
                    ayM = a;
                a = str.Results.FiniteElems.Reinforcement(slab.Number, fe.Number).AY_TOP;
                if (a > ayP)
                    ayP = a;
            }

            //getting results Fx, Fy, Fz, Mx, My, Mz for foot (node 1) with self-weight (case 1)
            txt += "Slab 1, Reinforcemet extreme values:" + "\n\r" + " Ax+ = " + axP * 10000 + " [cm2]" + "\n\r" + " Ax- = " +
                   axM * 10000 + " [cm2]" + "\n\r" + " Ay+ = " + ayP * 10000 + " [cm2]" + "\n\r" + " Ay- = " + ayM * 10000 +
                   " [cm2]" + "\n\r";

            foreach (var fe in finiteElementNodes)
            {
                var nodes = fe.Nodes;
                Console.WriteLine(
                    $"{fe.Number} - {Enumerable.Range(0, nodes.Count).Aggregate("", (x, y) => $"{x} {nodes.Get(y)}")}");
            }


            Console.WriteLine(txt);
        }

        private static void CreateNewProjectWithSlab()
        {
            var robot = new RobotApplication();

            robot.Project.New(IRobotProjectType.I_PT_SHELL);
            robot.Visible = 1;

            var preferences = robot.Project.Preferences;
            var meshParams = preferences.MeshParams;

            var strt = robot.Project.Structure;
            var labels = strt.Labels;

            robot.Project.Structure.Nodes.Create(1, 0, 0, 0);
            robot.Project.Structure.Nodes.Create(2, 3, 0, 0);
            robot.Project.Structure.Nodes.Create(3, 3, 3, 0);
            robot.Project.Structure.Nodes.Create(4, 0, 3, 0);

            var points = robot.Kernel.CmpntFactory.Create(IRobotComponentType.I_CT_POINTS_ARRAY) as RobotPointsArray;
            points.SetSize(4);

            points.Set(1, 0, 0, 0);
            points.Set(2, 3, 0, 0);
            points.Set(3, 3, 3, 0);
            points.Set(4, 0, 3, 0);

            var material = labels.Create(IRobotLabelType.I_LT_MATERIAL, "C30");
            var materialData = material.Data as IRobotMaterialData;
            materialData.Type = IRobotMaterialType.I_MT_CONCRETE;
            materialData.E = 30000000000;
            materialData.NU = 0.2;
            materialData.RO = 25000;
            materialData.Kirchoff = materialData.E / (2 * (1 + materialData.NU));
            labels.Store(material);

            var label = labels.Create(IRobotLabelType.I_LT_PANEL_THICKNESS, "Slab30");
            if (label.Data is RobotThicknessData thickness)
            {
                thickness.MaterialName = "C30";
                thickness.ThicknessType = IRobotThicknessType.I_TT_HOMOGENEOUS;
                ((IRobotThicknessHomoData)thickness.Data).ThickConst = 0.3;
            }

            labels.Store(label);

            var slabNumber = strt.Objects.FreeNumber;
            strt.Objects.CreateContour(slabNumber, points);
            RobotObjObject slab = null;
            if (strt.Objects.Get(slabNumber) is RobotObjObject)
            {
                slab = strt.Objects.Get(slabNumber) as RobotObjObject;
                slab.Main.Attribs.Meshed = 1;
                slab.SetLabel(IRobotLabelType.I_LT_PANEL_THICKNESS, "Slab30");
                slab.Initialize();
            }

            var supportLabel = labels.Create(IRobotLabelType.I_LT_SUPPORT, "Footer");
            var footData = supportLabel.Data as RobotNodeSupportData;
            footData.UX = 1;
            footData.UY = 1;
            footData.UZ = 1;
            footData.RX = 1;
            footData.RY = 1;
            footData.RZ = 1;
            labels.Store(supportLabel);

            var selection = strt.Selections.Get(IRobotObjectType.I_OT_NODE);
            selection.FromText("1 2 3 4");
            strt.Nodes.SetLabel(selection, IRobotLabelType.I_LT_SUPPORT, "Footer");

            var selfWeight = strt.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT,
                IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            selfWeight.Records.New(IRobotLoadRecordType.I_LRT_DEAD);

            var rec = selfWeight.Records.Get(1) as RobotLoadRecord;
            rec.SetValue((short)IRobotBarDeadRecordValues.I_BDRV_ENTIRE_STRUCTURE, 1);
            rec.SetValue((short)IRobotBarDeadRecordValues.I_BDRV_Z, -1);

            var liveLoad = strt.Cases.CreateSimple(2, "Live", IRobotCaseNature.I_CN_EXPLOATATION,
                IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            var uniform = liveLoad.Records.New(IRobotLoadRecordType.I_LRT_UNIFORM);
            var liveRec = liveLoad.Records.Get(uniform) as RobotLoadRecord;
            liveRec.SetValue((short)IRobotUniformRecordValues.I_URV_PX, 0);
            liveRec.SetValue((short)IRobotUniformRecordValues.I_URV_PY, 0);
            liveRec.SetValue((short)IRobotUniformRecordValues.I_URV_PZ, -1000);

            liveRec.Objects.FromText(slabNumber.ToString());

            preferences.SetActiveCode(IRobotCodeType.I_CT_CODE_COMBINATIONS, "BAEL 93");

            robot.Project.CalcEngine.Calculate();

            var nodeResults = robot.Project.Structure.Results.Nodes;

            Console.WriteLine(nodeResults.Reactions.Value(2, 1).FZ);
            Console.WriteLine(nodeResults.Reactions.Value(2, 2).FZ);
            Console.WriteLine(nodeResults.Reactions.Value(2, 3).FZ);
            Console.WriteLine(nodeResults.Reactions.Value(2, 4).FZ);

            var feSelection = strt.Selections.Get(IRobotObjectType.I_OT_FINITE_ELEMENT);
            feSelection.FromText(slab.FiniteElems);
            var feLabels = strt.FiniteElems.GetMany(feSelection);
        }

        private static void AnalyseSingleBeam()
        {
            var robot = new RobotApplication();

            robot.Project.New(IRobotProjectType.I_PT_FRAME_2D);

            robot.Project.Structure.Nodes.Create(1, 0, 0, 0);
            robot.Project.Structure.Nodes.Create(2, 3, 0, 0);

            robot.Project.Structure.Bars.Create(1, 1, 2);

            var label = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_SUPPORT, "Support");

            var supportData = label.Data as RobotNodeSupportData;
            supportData.UX = 1;
            supportData.UY = 1;
            supportData.UZ = 1;
            supportData.RX = 0;
            supportData.RY = 0;
            supportData.RZ = 0;

            robot.Project.Structure.Labels.Store(label);
            robot.Project.Structure.Nodes.Get(1).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");
            robot.Project.Structure.Nodes.Get(2).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");

            var barLabel = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "Beam50*50");
            var sectionData = barLabel.Data as RobotBarSectionData;

            sectionData.ShapeType = IRobotBarSectionShapeType.I_BSST_CONCR_BEAM_RECT;
            var concrete = sectionData.Concrete as RobotBarSectionConcreteData;

            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_B, 0.5);
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_H, 0.5);

            sectionData.CalcNonstdGeometry();

            robot.Project.Structure.Labels.Store(barLabel);
            robot.Project.Structure.Bars.Get(1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, "Beam50*50");

            var lc = robot.Project.Structure.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT,
                IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            lc.Records.New(IRobotLoadRecordType.I_LRT_DEAD);

            var rec = lc.Records.Get(1) as RobotLoadRecord;
            rec.SetValue((short)IRobotBarDeadRecordValues.I_BDRV_ENTIRE_STRUCTURE, 1);
            rec.SetValue((short)IRobotBarDeadRecordValues.I_BDRV_Z, -1);

            robot.Project.CalcEngine.Calculate();

            var result = robot.Project.Structure.Results.Bars.Forces.Value(1, 1, 0.5);

            Console.WriteLine($"{result.MX:F5} {result.MY:F5} {result.MZ:F5}");
        }
    }
}

/*
 * Created by SharpDevelop.
 * User: fabih
 * Date: 06/04/2021
 * Time: 09:39 a. m.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace ACABADOS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("445BECC2-363C-492C-91C5-9842E9D7BBFF")]
    public partial class ThisApplication
    {
        private void Module_Startup(object sender, EventArgs e)
        {

        }

        private void Module_Shutdown(object sender, EventArgs e)
        {

        }

        #region Revit Macros generated code
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(Module_Startup);
            this.Shutdown += new System.EventHandler(Module_Shutdown);
        }
        #endregion
        public void TAGWALLLAYERS()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            
            using(Transaction trans = new Transaction(doc,"WALLSTAG"))
            {
                trans.Start();
                Wall muro = doc.GetElement(sel.PickObject(ObjectType.Element)) as Wall;
                
                SetWallInfo(doc,muro);
                
                trans.Commit();
            }
            
        }
        
        private void SetWallInfo(Document doc, Wall wall)
        {
            WallType walltype = wall.WallType;
            
            string datos = "";
            
            if(WallKind.Basic == walltype.Kind)
            {
                CompoundStructure comStruct = walltype.GetCompoundStructure();
                Categories allCates = doc.Settings.Categories;
                
    
                Category wallCate = allCates.get_Item(BuiltInCategory.OST_Walls);
                Material wallMaterial = wallCate.Material;
                
                
                Parameter finish1a = walltype.LookupParameter("Finish1A-AXM");
                Parameter finish1 = walltype.LookupParameter("Finish1AXM");
                Parameter finish2a = walltype.LookupParameter("Finish2A-AXM");
                Parameter finish2 = walltype.LookupParameter("Finish2AXM");
                Parameter insulation = walltype.LookupParameter("InsulationAXM");
                Parameter structure = walltype.LookupParameter("StructureAXM");
                
                List<int> capasmurosIds = GetLayersIds(comStruct);
                IList<CompoundStructureLayer> layers = walltype.GetCompoundStructure().GetLayers();
                
                
                
                foreach(CompoundStructureLayer structLayer in comStruct.GetLayers())
                {
                    Material layerMat = doc.GetElement(structLayer.MaterialId) as Material;
                    Parameter par = layerMat.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                    
                    datos += structLayer.LayerId.ToString() + layerMat.Name +  Environment.NewLine;
        
                    if(null != layerMat)
                    {
                        switch(structLayer.Function)
                        {
                                case MaterialFunctionAssignment.None:
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsDefault).Material;
                                    structure.Set(par.AsString());
                                    break;
                                case MaterialFunctionAssignment.Structure:
                                    
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsStructure).Material;
                                    structure.Set(par.AsString());
                                    break;
                                case MaterialFunctionAssignment.Substrate:
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsSubstrate).Material;

                                    break;
                                case MaterialFunctionAssignment.Insulation:
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsInsulation).Material;
                                    //insulation.Set(par.AsString());
                                    break;
                                case MaterialFunctionAssignment.Finish1:
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsFinish1).Material;
        
                                    break;
                                case MaterialFunctionAssignment.Finish2:
                                    
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsFinish2).Material;
                                   
                                    break;
                                case MaterialFunctionAssignment.Membrane:
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsMembrane).Material;
                                    
                                    break;
                                case MaterialFunctionAssignment.StructuralDeck:
                                    layerMat = allCates.get_Item(BuiltInCategory.OST_WallsSurfacePattern).Material;
                                    
                                    break;
                                default:
                                    break;
                                
                        }
                        if(null == layerMat)
                        {
                            layerMat = wallMaterial;
                        }
                    }
                    
                }
                TaskDialog.Show("REVIT" , datos );
            
            
            }
        }
        
        private List<int> GetLayersIds(CompoundStructure cstruct)
        {
            List<int> indexes = new List<int>();
            
            foreach(CompoundStructureLayer st in cstruct.GetLayers())
            {
                indexes.Add(st.LayerId);
                
            }
            
            return indexes;
        }
    }
}
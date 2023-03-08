/*
 * Created by SharpDevelop.
 * User: arturo.rodriguez
 * Date: 12/07/2022
 * Time: 07:01 p. m.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace nuevos
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("02F7A548-DFCE-4F79-A9D4-D80E3DD908B2")]
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
		public void hola()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			TaskDialog.Show("INFO","HOLA!!!");
		}
		public void SelectByCategory()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			ICollection<ElementId> elements = new FilteredElementCollector(doc,doc.ActiveView.Id).OfClass(typeof(ViewSection)).ToElementIds();
			uidoc.Selection.SetElementIds(elements);
		}
		public void DuplicandoPlanos()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			Selection sel = uidoc.Selection;
			
			ICollection<ElementId> sheetsIds = sel.GetElementIds();
			
			DuplicateCommand(doc,sheetsIds);

		}
		
		public void DuplicateCommand(Document doc, ICollection<ElementId> sheetIds)
		{
			ElementId ttbid = new FilteredElementCollector(doc)
				.OfCategory(BuiltInCategory.OST_TitleBlocks)
				.WhereElementIsElementType()
				.FirstOrDefault()
				.Id;
			
			int contador= 0;
			using(Transaction trans = new Transaction(doc,"Duplicate Sheets"))
			{
				
				try
				{
					trans.Start();
					foreach(ElementId eleid  in sheetIds)
					{
						contador +=1;
						ViewSheet vsit = doc.GetElement(eleid) as ViewSheet;
						ICollection<ElementId>viewportId = vsit.GetAllViewports();
						
						string name = vsit.Name + "Copy" + contador.ToString();
						string shnumber = "(CP)-" + vsit.SheetNumber+contador;
						
						//Parameter Folder = vsit.LookupParameter("Folder");
						//Parameter folder = vsit.LookupParameter("folder");
						
						ViewSheet vsi = ViewSheet.Create(doc,ttbid);
						vsi.Name = name;
						vsi.SheetNumber = shnumber;
						//Parameter _Folder = vsi.LookupParameter("Folder");
						//Parameter _folder = vsi.LookupParameter("folder");
						
						//_Folder.Set(Folder.AsString());
						//_folder.Set(folder.AsString());
						        
						foreach(ElementId elid in viewportId)
						{
							Viewport vp = doc.GetElement(elid) as Viewport;
							View vista = doc.GetElement(vp.ViewId) as View;
							XYZ cpoint = vp.GetBoxCenter();
							
							
							ElementId newviewId = vista.Duplicate(ViewDuplicateOption.WithDetailing);
							View newView = doc.GetElement(newviewId) as View;
							
							newView.Name = vista.Name + "-(COPY)" + contador;
							
							Viewport newvp = Viewport.Create(doc,vsi.Id,newviewId,cpoint);
							
						}
						
					}
					
					TaskDialog.Show("INFO","TAREA FINALIZADA CON EXITO");
				}
				catch(Exception e)
				{
					TaskDialog.Show("Warning!","ALGO SUCEDIO" + e.Message);
					trans.RollBack();
				}
				
				trans.Commit();
				
			}
			
		}
		public void ViewsheettoViews()
		{
		}

		public void FamilyGenerator()
		{
			// Global Parameters
			UIApplication uiapp = new UIApplication(this.Application);
			Document doc = this.ActiveUIDocument.Document;
			string templateFileName = @"C:\ProgramData\Autodesk\RVT 2019\Family Templates\English_I\Generic Model.rft";
			
			DWGImportOptions importOpt = new DWGImportOptions();
			importOpt.Placement = ImportPlacement.Centered;
			importOpt.Unit = ImportUnit.Meter;
			ElementId eleid = null;
			
			//DIRECTORY AND PATH OPERATIONS
			string[] files = Directory.GetFiles(@"C:\Users\arturo.rodriguez\Downloads\RESTAURANTES\CADS");
			List<Document> familydocs = new List<Document>();
			List<string>filepaths = new List<string>();
			List<string> cadfiles = new List<string>();
			/// <summary>
			/// 
			/// <MOMENTO DE ITERAR POR CADA ARCHIVO CAD EN UN FOLDER>
			/// 
			//int index = 0;
			foreach(string file in files)
			{
				cadfiles.Add(file);
				Document famDoc = this.Application.NewFamilyDocument(templateFileName);
				string cadTemplatefilename = Path.GetFileName(file);
				int charCount = cadTemplatefilename.Length - 4;
				string newname = cadTemplatefilename.Remove(charCount,4) + ".rfa";
				
				
				string ruta = @"C:\Users\arturo.rodriguez\Downloads\RESTAURANTES\FAMILIAS";
				string newfilewithpath = Path.Combine(ruta,newname);
				filepaths.Add(newfilewithpath);
				
				famDoc.SaveAs(newfilewithpath);
				familydocs.Add(famDoc);
					
			}
			
			for(int i = 0; i < cadfiles.Count(); i++)
			{
				UIDocument famuidoc = uiapp.OpenAndActivateDocument(filepaths[i]);
				View vista = famuidoc.Document.ActiveView;
				Document doco = familydocs[i];
				using(Transaction trans = new Transaction(doco,"ImportInstance"))
				{
					trans.Start();
					doco.Import(cadfiles[i], importOpt,vista, out eleid);
					trans.Commit();
				}
			}
	

		}
		public void DirectoryManagement()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			string[] files = Directory.GetFiles(@"C:\Users\arturo.rodriguez\Downloads\220921 BIND\CADS TO FAMS");
			string plot = "";
			
			foreach(string file in files)
			{
				string fileName = Path.GetFileName(file);
				int charCount = fileName.Length - 4;
				string newname = fileName.Remove(charCount,4);
				
				plot += newname + Environment.NewLine;
				
			}
			
			//int cantidad = files.Count();
			
			TaskDialog.Show("INFO",plot);
				
				
			
		}
		

		public void FlipSelectedWall()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			ISelectionFilter esl = new WallSelectionFilter();
			Selection sel = uidoc.Selection;
			IList<Reference> refs = sel.PickObjects(ObjectType.Element,esl,"SELECCIONAR MUROS");
			
			string message = "";
			using(Transaction t = new Transaction(doc,"flipando muros"))
			{
				try
				{
					t.Start();
					
					foreach(Reference refe in refs)
					{
						Wall muro = doc.GetElement(refe) as Wall;
						muro.Flip();
						if(muro.Flipped)
						{
							message += "LOS SIGUIENTES MUROS SE HAN FLIPADO";
						}
						else{
							message += "ALGO SUCEDIO!";
						}
					}
				}
				catch(Exception e)
				{
					TaskDialog.Show("WARNING!","ALGO SUCEDIO!" + e.Message);
					t.RollBack();
				}
				t.Commit();
			}
		}
		
		public class WallSelectionFilter : ISelectionFilter
		{
			public bool AllowElement(Element ele)
			{
				if(ele.Category.Name == "Walls")
				{
					return true;
				}
				else{
					return false;
				}
			}
			
			public bool AllowReference(Reference refe, XYZ point)
			{
				return false;
			}
			
		}
		
		public void ViewCropping()
		{
			UIDocument UIDOC = this.ActiveUIDocument;
			Document doc = UIDOC.Document;
			
		}
		
		private void CropAroundRoom(Room room, View view)
		{
			if(view != null)
			{
				IList<IList<BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
				
				if(null != segments)
				{
					foreach(IList<BoundarySegment> segmentList in segments)
					{
						CurveLoop clop = new CurveLoop();
						foreach(BoundarySegment bs in segments)
						{
							clop.Append(bs.GetCurve());
						}
						
						ViewCropRegionShapeManager vsm = view.GetCropRegionShapeManager();
						vsm.SetCropShape(clop);
						break;
					}
				}
			}
		}
		public void deletecads()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(ImportInstance));
			
			string info = "";
			
			IList<ElementId> eleids = new List<ElementId>();
			foreach(ImportInstance cad in collector)
			{
				ElementId eid = cad.Id;
				eleids.Add(eid);
				
				info += cad.Id + Environment.NewLine;
			}
			
			using(Transaction trans = new Transaction(doc,"BORRANDO CADS"))
			{
				trans.Start();
				
				doc.Delete(eleids);
				
				trans.Commit();
			}
			
			TaskDialog.Show("INFO","CADS BORRADOS : " + Environment.NewLine + " - " + info);
		}
	}
}
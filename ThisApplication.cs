/*
 * Created by SharpDevelop.
 * User: arturo.rodriguez
 * Date: 14/01/2023
 * Time: 02:10 p. m.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

using Autodesk.Revit.Exceptions;

using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace DATA
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("0349C04F-5961-4ABF-9FD6-818D6C4AA901")]
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
		public void JOINELEMENTSBYCATEGORIES()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			FilteredElementCollector cates1 = new FilteredElementCollector(doc,doc.ActiveView.Id)
				.OfClass(typeof(FamilyInstance))
				.OfCategory(BuiltInCategory.OST_StructuralFraming);
			
			foreach(FamilyInstance scolumn in cates1)
			{
				FilteredElementCollector cates2  =  new FilteredElementCollector(doc,doc.ActiveView.Id)
					.OfClass(typeof(Floor)).WhereElementIsNotElementType();
				cates2.WherePasses( new ElementIntersectsElementFilter(scolumn));
				
				foreach(Floor losa in cates2)
				{
					using(Transaction trans = new Transaction(doc,"JOINING"))
					{
						trans.Start();
						
						FailureHandlingOptions opts = trans.GetFailureHandlingOptions();
						WarningDiscard preprocessor = new WarningDiscard();
						opts.SetFailuresPreprocessor(preprocessor);
						opts.SetClearAfterRollback(true);
						trans.SetFailureHandlingOptions(opts);
						
						try
						{
							bool check = JoinGeometryUtils.AreElementsJoined(doc,scolumn,losa);
							if(check)
							{
								JoinGeometryUtils.SwitchJoinOrder(doc,scolumn,losa);
							}
							else
							{
								JoinGeometryUtils.JoinGeometry(doc,scolumn,losa);
							}
						}
						catch(Exception e)
						{
							TaskDialog.Show("ERROR",e.Message);
						}
						
						
						
						trans.Commit();
					}
				}
			}
		}
		public class UniversalFailureHandler : IFailuresPreprocessor
		{
			public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor) 
			{
				
				failuresAccessor.DeleteAllWarnings();
				
				
				return FailureProcessingResult.Continue;
			}
		}
		
		
		
		
		public class WarningDiscard :IFailuresPreprocessor
		{
			
			public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
			{
				string transactionName = failuresAccessor.GetTransactionName();
				IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();
				
				if(fmas.Count == 0)
				{
					return FailureProcessingResult.Continue;
				}
				bool hasError = false;
				
				foreach(FailureMessageAccessor fm in fmas)
				{
					if(fm.GetFailureDefinitionId() == BuiltInFailures.JoinElementsFailures.CannotCutJoinedGeometry 
					   || fm.GetFailureDefinitionId() == BuiltInFailures.JoinElementsFailures.CannotJoinElements 
					   ||  fm.GetFailureDefinitionId() == BuiltInFailures.JoinElementsFailures.CannotJoinElementsError
					   || fm.GetFailureDefinitionId() == BuiltInFailures.JoinElementsFailures.CannotKeepJoined
					   || fm.GetFailureDefinitionId() == BuiltInFailures.JoinElementsFailures.CannotJoinElementsWarn)
					{
						if(fm.HasResolutions())
						{
							failuresAccessor.DeleteWarning(fm);
							hasError = true;
						}
					}
				}
				if(hasError)
                {
                    return FailureProcessingResult.ProceedWithRollBack;
                }

				
				return FailureProcessingResult.Continue;
				
			}
		}
		public void noWRNMAKELINES()
		{
			Document doc = this.ActiveUIDocument.Document;
			using(Transaction t = new Transaction(doc,"MAKE LINES"))
			{
				t.Start();
				
				
				
				t.Commit();
			}
		}
		
		public void MakeRoom()
		{
			Document doc = this.ActiveUIDocument.Document;
			using(Transaction t = new Transaction(doc,"MAKE ROOMS"))
			{
				t.Start();
				
				
				t.Commit();
			}
		}
		
		
		public void ScheduleCreate()
		{
			//VARIABLES GLOBALES
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			
			//CONCENTRANDO PARAMETROS
			BuiltInParameter[] BiParams = new BuiltInParameter[] 
			{
				BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM,
				BuiltInParameter.ELEM_CATEGORY_PARAM,
				BuiltInParameter.WALL_BASE_CONSTRAINT,
				BuiltInParameter.HOST_AREA_COMPUTED,
				BuiltInParameter.WALL_BASE_CONSTRAINT
			};
			
			using(Transaction trans = new Transaction(doc,"CREANDO TABLAS"))
			{
				trans.Start();
				
				
				
				trans.Commit();
			}
			
		}
		
		private bool CheckField(SchedulableField vs)
		{
			///foreach(BuiltInParameter bip in BiParams
			/// 
			return true;
		}
	}
}
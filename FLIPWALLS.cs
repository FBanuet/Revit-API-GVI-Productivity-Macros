		public void FlipSelectedWalls()
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
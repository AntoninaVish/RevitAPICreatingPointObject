using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Prism.Commands;
using RevitAPIPointObjectsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPICreatingPointObject
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData; //записываем в поле _commandData

        public Pipe Pipe { get; }

        public List<FamilySymbol> FamilyTypes { get; } = new List<FamilySymbol>();

        public DelegateCommand SaveCommand { get; }

        public FamilySymbol SelectedFamilyType { get; set; }


        public MainViewViewModel(ExternalCommandData commandData)//конструктор
        {
            _commandData = commandData;
            Pipe = SelectionUtils.GetObject<Pipe>(commandData, "Выберите трубу");
            FamilyTypes = FamilySymbolUtils.GetFamilySymbols(commandData); //FamilySymbol- мы собираем все типы семейств которые существуют в моделе

            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var locationCurve = Pipe.Location as LocationCurve;//находим (извлекаем) LocationCurve, которая понадобится для извлечения осевой линии нашей трубы
            var pipeCurve = locationCurve.Curve;//извлекаем эту осевую линию трубы

            var oLevel = (Level)doc.GetElement(Pipe.LevelId);//извлекаем уровень из трубы ( уровень собран)

            FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedFamilyType, pipeCurve.GetEndPoint(0), oLevel); //создаем точечные семейства, начало трубы
            FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedFamilyType, pipeCurve.GetEndPoint(1), oLevel); //создаем точечные семейства, конц трубы

            RaiseCloseRequest();//запускаем метод, который закроет окно после выполнения программы

        }

        public event EventHandler CloseRequest;//определили event

        private void RaiseCloseRequest() //метод для закрытия окна
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
            

    }
}

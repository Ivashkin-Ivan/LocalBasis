using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.UI;
using LocalBasis.Model;
using LocalBasis.View;
using LocalBasis.ViewModel;
namespace LocalBasis.ExternalCommand
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class RevitCommand : IExternalCommand
    {
        private Document _doc;
        private Element _floor;
        private Autodesk.Revit.DB.View _view;
        private List<BoundingBoxXYZ> listBB { get; set; } = new List<BoundingBoxXYZ>();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            var doc = commandData.Application.ActiveUIDocument.Document;
            var _view = commandData.View;
            _doc = doc;
            _floor = new FilteredElementCollector(_doc).OfClass(typeof(Floor)).First();
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Safety transaction");


                //Разобрать данный код позже
                #region

                //1) Имеется ревит с элементами,
                //2) Размещаю куб (зашить isAcivae в MathCore)
                //3) Выделяю элементы рамкой, ставлю ElementFilter, чтобы выбрал только asLine и asInstance
                //4) В цикле создаю для них CustomVector и CustomInstance
                //5) В цикле генерирую для них системы координат
                //6) Беру элемент, тяну view inst or vect в зависимости от выбранного элемента
                //7) В VM передаю выбранный элемент (таскаю элемент, жму Track, на выход получаю таблицу координат
                //8) Пишу в Custom-ах метод, который генрирует таблицу Basis + GlobalOrigin







                // Очень важный код, разработака пересчёта Basis-а + тестовый id из документа стены
                /*


                FamilyInstance cube = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                                                                       .Cast<FamilyInstance>()
                                                                       .Last(it => it.Symbol.FamilyName == "RedCube" && it.Symbol.Name == "Красный");

                Element duct = new FilteredElementCollector(doc).OfClass(typeof(Duct)).Cast<Duct>().First() as Element;

                //var angle = Math.PI / 4; // Угол, на который будет происходить поворот

                //Transform transform1 = Transform.CreateRotation(XYZ.GlobalBasisZ, angle);

                // XYZ vector = new XYZ(0, 1000, 0); //Пока не понятно это смещение или новые координаты //Это смещение

                //Transform transform2 = Transform.CreateTranslation(vector);

                //Transform compose1 = transform1 * transform2;
                //Transform compose2 = transform2 * transform1;

                // Трансформ в минус первой стпени


                //Трубы для проверки пересчёта
                Element duct1 = doc.GetElement(new ElementId(287225));
                Element duct2 = doc.GetElement(new ElementId(287240));
                var vector1 = ((duct1.Location as LocationCurve).Curve as Line).Direction;
                var vector1Origin = ((duct1.Location as LocationCurve).Curve as Line).GlobalOrigin;
                var vector2 = ((duct2.Location as LocationCurve).Curve as Line).Direction;
                var vector2Origin = ((duct2.Location as LocationCurve).Curve as Line).GlobalOrigin;
                //Инстанцы для проверки пересчёта
                Element cube1 = doc.GetElement(new ElementId(288349));
                Element cube2 = doc.GetElement(new ElementId(288614));
                var tcube1 = (cube1 as Instance).GetTotalTransform();
                var tcube2 = (cube2 as Instance).GetTotalTransform();

                var tcube1Origin = (cube1 as Instance).GetTotalTransform().GlobalOrigin;
                var tcube2Origin = (cube2 as Instance).GetTotalTransform().GlobalOrigin;

                var cube1Origin = (cube1.Location as LocationPoint).Point;
                var cube2Origin = (cube2.Location as LocationPoint).Point;

                // Получим из трубы направляющий вектор
                var vector = ((duct.Location as LocationCurve).Curve as Line).Direction;
                var origin = ((duct.Location as LocationCurve).Curve as Line).GlobalOrigin;

                // Создадим трансформ по этому вектору


                var listid = new List<ElementId>(); // Эти строки в движке лишние
                listid.Add(cube.Id);                // Эти строки в движке лишние

                var fo = cube.FacingOrientation;

                double angle1; //Дают одинаковый результат
                double angle2; //Дают одинаковый результат

                if (vector.X > 0)
                {
                    angle1 = 2 * Math.PI - vector.AngleTo(fo); //Угол между направляющим и FacingOrientation
                    angle2 = 2 * Math.PI - fo.AngleTo(vector); //Угол между направляющим и FacingOrientation
                }
                else
                {
                    angle1 = vector.AngleTo(fo); //Угол между направляющим и FacingOrientation
                    angle2 = fo.AngleTo(vector); //Угол между направляющим и FacingOrientation
                }

                    Line rotationAxis = Line.CreateBound(origin, origin + new XYZ(0, 0, 1));

                    (cube.Location as LocationPoint).Point = origin;
                    (cube.Location as LocationPoint).Rotate(rotationAxis, angle1);
                    // Рассмотрим плоский xOy случай, повернём вокруг ориджина на угол взятый из метода AngleTo(),
                    // Обобщ модель (типо система координат, примагничивается в трубе и поворачивается на нужный угол, при условии, что первоначальная
                    // ориентация куба по базису doc, поэтому, если куб был повёрнут, то система локальная система координат НЕ будет верно настроена



                    //====================================================//Получение прямого и обратного преобразования
                    var instance = cube as Instance;
                    var transform = instance.GetTotalTransform();       // Матрица
                    var inverse = instance.GetTotalTransform().Inverse; //Обратная матрица
                    //====================================================



                    //====================================================//Пересчёт вектора (по сути любого элемента основанного на направляющем векторе
                    var vector2New = inverse.OfVector(vector2);                     // Данная строка преобразует координаты вектора из общего базиса в координаты локального базиса 
                    var vector2Return = transform.OfVector(vector2New);             // Данная строка преобразует координаты вектора из локаьного базиса в общий базис
                    var vector2NewOrigin = inverse.OfPoint(vector2Origin);          // origin через Locaion из общего в локальный
                    var vector2ReturnOrigin = transform.OfPoint(vector2NewOrigin);  // origin через Locaion из локального в общий
                    //==================================================== 



                    //====================================================//Пересчёт элемента, который можно предстваить как instance

                    // Так как Revit матрица считается кватрнионной и работает в R^4, а размерность локального базиса R^3, то линейная комбинация последнего столбца GlobalOrigin работает некорректно
                    // в произвдении матриц, приходится вручную брать метод OfPoint и переписывать данный столбец дополнитльным действием. В debug-е GlobalBasisX, GlobalBasisY, GlobalBasisZ
                    // можно рассматривать как матрицу 3х3, а строку GlobalOrigin представлять как столбец приписанный к этой матрице справа, 4 строка заполняется нулями, в месте (4;4) стоит единица

                    var tcube2New = tcube2 * inverse;
                    tcube2New.GlobalOrigin = inverse.OfPoint(tcube2.GlobalOrigin); //Данные две строки преобразуют intance из общего базиса в локальный базис


                    var tcube2Return = tcube2New * transform;
                    tcube2Return.GlobalOrigin = transform.OfPoint(tcube2New.GlobalOrigin); //Данные две строки пробразуют insance из локального базиса в общий базис

                    // Также можно пересчитывать GlobalOrigin через Location, хотя код выше уже учитывает пересчёт GlobalOrigin

                    var cube2NewOrigin = inverse.OfPoint(cube2Origin);          // origin через Locaion из общего в локальный
                    var cube2ReturnOrigin = transform.OfPoint(cube2NewOrigin);  // origin через Locaion из локального в общий                                                                              
                                                                                //====================================================


                    

                    */





                //Очень важный код ============================================
                //Логика про sin здания



                /*var list = new List<Element>();
                var dict = new Dictionary<Element, Transform>();
                //var listTuple = new List<Tuple<Element, Transform, List<BoundingBoxXYZ()>>();
              list = new FilteredElementCollector(doc).OfClass(typeof(Wall)).ToElements().ToList();
                foreach(var e in list)
                {
                    var mc = new MathCore();
                    var found = (e.Location as LocationCurve).Curve as Line;
                    var system = mc.CreateLocalCoordinateSystem(found, doc);
                    dict.Add(e,system);
                }
                var windows = new FilteredElementCollector(doc)
                   .WhereElementIsNotElementType()
                   .OfCategory(BuiltInCategory.OST_Windows)
                   .Cast<FamilyInstance>()
                   .ToList();

                foreach(var e in windows)
                {
                    listBB.Add(e.get_BoundingBox(view));
                }
                //Теперь мы имеет словарь с вектором и местной системой

                //=====================================================
                //логика эталонного случая в методе
                List<FamilyInstance> MakeSituation(Element wall, Transform system) //добавить лист BB окон относящихся к данной стене
                {
                    var listFamilyInstance = new List<FamilyInstance>();
                    var wallLine = (wall.Location as LocationCurve).Curve as Line;
                    var level = _doc.GetElement(wall.LevelId) as Level;

                    var listId = new List<ElementId>();
                    var height = wall.LookupParameter("Неприсоединенная высота").AsDouble();
                    for (double z = 0; z < height; z += 1)
                    {
                        for (double y = 0; y < wallLine.ApproximateLength; y += 1)
                        {
                            XYZ checkPoint = new XYZ(0, y, z); //Точка, которая ползёт по стене;
                            XYZ familyLocation = new XYZ(4 * Math.Sin(0.2 * y) - 5, y, z);
                            FamilySymbol familySymbol = new FilteredElementCollector(_doc).OfClass(typeof(FamilySymbol))
                                                                            .OfCategory(BuiltInCategory.OST_GenericModel)
                                                                            .Cast<FamilySymbol>()
                                                                            .First(it => it.FamilyName == "RedCube" && it.Name == "Красный");

                            var locationinGlobal = system.OfPoint(familyLocation);
                            if (!familySymbol.IsActive)
                            {
                                familySymbol.Activate();
                            }

                            var fi = _doc.Create.NewFamilyInstance(locationinGlobal, familySymbol, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            listId.Add(fi.Id);
                            var axis = Line.CreateUnbound(locationinGlobal, new XYZ(
                                                                                  locationinGlobal.X,
                                                                                  locationinGlobal.Y,
                                                                                  locationinGlobal.Z + 1
                                                                                 ));
                            //ElementTransformUtils.RotateElement(_doc,fi.Id,axis,Math.PI/4);

                            foreach (var bb in listBB)
                            {
                                if (checkPoint.Y < bb.Min.Y)
                                {
                                    continue;
                                }
                                if (
                                    checkPoint.Y >= bb.Min.Y &&
                                    checkPoint.Y <= bb.Max.Y &&
                                    checkPoint.Z >= bb.Min.Z &&
                                    checkPoint.Z <= bb.Max.Z
                                    )
                                {
                                    //listId.Remove(fi.Id);
                                    //_doc.Delete(fi.Id);
                                }
                            }
                            listFamilyInstance.Add(fi);

                        }
                    }
                     
                    return listFamilyInstance;
                }

                //=====================================================

                foreach(var pair in dict)
                {
                    
                    
                    var listfi = MakeSituation(pair.Key, pair.Value); //сюда подавать в нужной системе координат
                    
                   

                    //пересчёт координат для листа фэмили инстанц
                    //создание фэмили инстанц
                }*/

                #endregion
                //==========================


                //Плагин хомуты

                var listPipe = new List<Element>();
                var list = new List<Element>();
                var dict = new Dictionary<Element, Transform>();
                list = new FilteredElementCollector(doc).OfClass(typeof(Duct)).ToElements().ToList();      // Лучше сделать через ElementFilter
                listPipe = new FilteredElementCollector(doc).OfClass(typeof(Pipe)).ToElements().ToList();
                foreach (var e in listPipe)
                {
                    list.Add(e);
                }

                foreach(var e in list)
                {
                    var mc = new MathCore();
                    var found = (e.Location as LocationCurve).Curve as Line;
                    if (found.Direction.Z <= -1E-1 || found.Direction.Z >= 1E-1)
                        continue;
                    var system = mc.CreateLocalCoordinateSystem(found, doc);
                    dict.Add(e,system);
                }

                //Теперь мы имеет словарь с вектором и местной системой

                foreach(var pair in dict)
                {                       
                    var listfi = MakeSituation(pair.Key, pair.Value); //сюда подавать в нужной системе координат
                }

                //View and VM
                #region
                //Для трэкера
                //Создадим систему на основе красного куба
                /*var elementForCoord = doc.GetElement(new ElementId(286188));
                var instanceForCoord = elementForCoord as Instance;
                var mc2 = new MathCore();
                var localSystem = mc2.CreateLocalCoordinateSystem(instanceForCoord, doc);


                var element = doc.GetElement(new ElementId(288349)); //Элемент который будет трэкать
                var model = new CustomInstance(element, localSystem);
                


                var vm = new InstanceViewModel();                
                vm.CustomInstance = model;
                var ui = new InstanceView();
                ui.DataContext = vm;
                ui.Show();*/
                #endregion

                transaction.Commit();
            }
            return Result.Succeeded;
        }

        
        List<FamilyInstance> MakeSituation(Element duct, Transform system)
        {
            var listFamilyInstance = new List<FamilyInstance>();
            var ductLine = (duct.Location as LocationCurve).Curve as Line;
            var level = _doc.GetElement(duct.LevelId) as Level;

            var listId = new List<ElementId>();
            if (duct.LookupParameter("Внешний диаметр") != null)
            {
                return null;
            }
            for (double y = 0; y < ductLine.ApproximateLength; y += 2)
            {
                XYZ familyLocation = new XYZ(0, y, 0);
                FamilySymbol familySymbol = new FilteredElementCollector(_doc).OfClass(typeof(FamilySymbol))
                                                                .OfCategory(BuiltInCategory.OST_GenericModel)
                                                                .Cast<FamilySymbol>()
                                                                .First(it => it.FamilyName == "HILTI_s_Хомут_MPN-RC" && it.Name == "Хомут для воздуховодов MV-PI");
                var locationinGlobal = system.OfPoint(familyLocation); //Домножил Location на S^(-1) для возврата в систему ревит
                if (!familySymbol.IsActive)
                {
                    familySymbol.Activate();
                }
                var fi = _doc.Create.NewFamilyInstance(locationinGlobal, familySymbol, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                var axis = Line.CreateBound(locationinGlobal, new XYZ(
                                              locationinGlobal.X,
                                              locationinGlobal.Y,
                                              locationinGlobal.Z + 1
                                             ));
                var angle = system.BasisY.AngleTo(fi.FacingOrientation); //сооринтровал по местной системе координат без возврата
                ElementTransformUtils.RotateElement(_doc, fi.Id, axis, angle);
                double value = 1000;
                try
                {
                    if (duct.LookupParameter("Внешний диаметр") != null)
                        value = duct.LookupParameter("Внешний диаметр").AsDouble();
                    else if (duct.LookupParameter("Диаметр") != null)
                        value = duct.LookupParameter("Диаметр").AsDouble();
                    fi.LookupParameter("ADSK_Размер_Диаметр").Set(value);
                    if (fi.LookupParameter("ADSK_Размер_Диаметр").AsDouble() > (800 / 304.8))
                    {
                        _doc.Delete(fi.Id);
                    }
                }
                catch
                {

                }

                //Притягивание к перекрытию

                var bottomRef = HostObjectUtils.GetBottomFaces(_floor as Floor).First();
                var bottomFace = (Face)(_floor as Floor).GetGeometryObjectFromReference(bottomRef);
                var projectResult = bottomFace.Project(fi.get_BoundingBox(_view).Max);
                if (projectResult != null)
                {
                    var lenght = _floor.get_BoundingBox(_view).Min.Z - (fi.Location as LocationPoint).Point.Z;
                    fi.LookupParameter("Длина шпильки").Set(lenght);
                }
                listId.Add(fi.Id);
                listFamilyInstance.Add(fi);

            }
            return listFamilyInstance;
        }
    }
}

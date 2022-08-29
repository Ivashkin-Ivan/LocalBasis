using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.UI;
using LocalBasis.Model;

namespace LocalBasis.ExternalCommand
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            var doc = commandData.Application.ActiveUIDocument.Document;

            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Safety transaction");




            // Очень важный код, разработака пересчёта Basis-а + тестовый id из документа стены

            #region

            FamilyInstance cube = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                                                                   .Cast<FamilyInstance>()
                                                                   .Last(it => it.Symbol.FamilyName == "RedCube" && it.Symbol.Name == "Красный");

            Element duct = new FilteredElementCollector(doc).OfClass(typeof(Duct)).Cast<Duct>().First() as Element;
           
            //var angle = Math.PI / 4; // Угол, на который будет происходить поворот

            //Transform transform1 = Transform.CreateRotation(XYZ.BasisZ, angle);

            // XYZ vector = new XYZ(0, 1000, 0); //Пока не понятно это смещение или новые координаты //Это смещение

            //Transform transform2 = Transform.CreateTranslation(vector);

            //Transform compose1 = transform1 * transform2;
            //Transform compose2 = transform2 * transform1;

            // Трансформ в минус первой стпени
       

            //Трубы для проверки пересчёта
            Element duct1 = doc.GetElement(new ElementId(287225));
            Element duct2 = doc.GetElement(new ElementId(287240));
            var vector1 = ((duct1.Location as LocationCurve).Curve as Line).Direction;
            var vector1Origin = ((duct1.Location as LocationCurve).Curve as Line).Origin;
            var vector2 = ((duct2.Location as LocationCurve).Curve as Line).Direction;
            var vector2Origin = ((duct2.Location as LocationCurve).Curve as Line).Origin;
            //Инстанцы для проверки пересчёта
            Element cube1 = doc.GetElement(new ElementId(288349));
            Element cube2 = doc.GetElement(new ElementId(288614));
            var tcube1 = (cube1 as Instance).GetTotalTransform();
            var tcube2 = (cube2 as Instance).GetTotalTransform();

            var tcube1Origin = (cube1 as Instance).GetTotalTransform().Origin;
            var tcube2Origin = (cube2 as Instance).GetTotalTransform().Origin;

            var cube1Origin = (cube1.Location as LocationPoint).Point;
            var cube2Origin = (cube2.Location as LocationPoint).Point;

            // Получим из трубы направляющий вектор
            var vector = ((duct.Location as LocationCurve).Curve as Line).Direction;
            var origin = ((duct.Location as LocationCurve).Curve as Line).Origin;

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

                // Так как Revit матрица считается кватрнионной и работает в R^4, а размерность локального базиса R^3, то линейная комбинация последнего столбца Origin работает некорректно
                // в произвдении матриц, приходится вручную брать метод OfPoint и переписывать данный столбец дополнитльным действием. В debug-е BasisX, BasisY, BasisZ
                // можно рассматривать как матрицу 3х3, а строку Origin представлять как столбец приписанный к этой матрице справа, 4 строка заполняется нулями, в месте (4;4) стоит единица

                var tcube2New = tcube2 * inverse;
                tcube2New.Origin = inverse.OfPoint(tcube2.Origin); //Данные две строки преобразуют intance из общего базиса в локальный базис


                var tcube2Return = tcube2New * transform;
                tcube2Return.Origin = transform.OfPoint(tcube2New.Origin); //Данные две строки пробразуют insance из локального базиса в общий базис

                // Также можно пересчитывать Origin через Location, хотя код выше уже учитывает пересчёт Origin

                var cube2NewOrigin = inverse.OfPoint(cube2Origin);          // origin через Locaion из общего в локальный
                var cube2ReturnOrigin = transform.OfPoint(cube2NewOrigin);  // origin через Locaion из локального в общий                                                                              
                                                                            //====================================================


                #endregion



                //Test MathCore

                var foundation = (duct.Location as LocationCurve).Curve as Line;
              
                var newCoord = MathCore.CreateLocalCoordinateSystem(foundation, doc);

                





              









                transaction.Commit();
            }



            return Result.Succeeded;

        }
    }
}

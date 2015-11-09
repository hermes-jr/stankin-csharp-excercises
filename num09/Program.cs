using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace conapp_test
{
	// Константы для удобства (вместо #define в Си)
	class Constants
	{
		// Для классов
		public const ushort MAX_RND = 10;

		// Для задачи
		public const ushort PARAM1 = 10; // Радиус круга по условию задачи
		public const ushort TOTAL_POINTS = 12; // Количество обрабатываемых точек плоскости
	}

	/* Класс базовой матрицы, при наследовании позволяет создать матрицу
	 * произвольного размера, заполненную превдослучайными целыми числами
	 * (подходит для большинства задач, в некоторых возможно нужно будет
	 * изменить тип на float/double)
	 */
	public class MatrixBasic
	{
		// Пара свойств, по условию задачи
		protected ushort Rows { get; set; }
		protected ushort Columns { get; set; }

		// Двумерный прямоугольный массив, в котором будут храниться элементы
		public double[,] Elements;

		// Индексатор, по условию задачи
		public double this[int row, int col]
		{
			get
			{
				return Elements[row, col];
			}
			set
			{
				Elements[row, col] = (double) value;
			}
		}

		// Конструктор класса по умолчанию
		public MatrixBasic()
		{
			this.Rows = 3;
			this.Columns = 2;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixBasic(ushort rows, ushort cols)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = (cols != 0) ? cols : (ushort) 2;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		/* Заполнение матрицы псевдослучайными целыми числами (элементы
		 * в диапазоне от -MAX_RND до MAX_RND включительно). В зависимости от
		 * условия задачи тут могут быть изменены типы данных
		 */
		public void Populate()
		{
			Random random = new Random();
			/* Бежим по матрице слева направо сверху вниз (большинство циклов
			 * будут реализованы так)
			 */
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					// ~случайный выбор знака + или -
					int sign = (int) Math.Pow(-1, random.Next(2));
					// ~случайный выбор значения элемента и тут же присвоение знака
					int currentNumber = random.Next(Constants.MAX_RND) * sign;

					this.Elements[iterR, iterC] = currentNumber;
				}
			}
		}

		// Вывод матрицы в виде таблицы
		public void Print()
		{
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				System.Console.Write('|'); // Левая граница, для красоты
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					/* Форматирование: N+1 символов в ширину (максимум N-значное число
					 * и знак. Если надо - увеличить). Пробел справа от числа и правая
					 * граница столбца
					 */
					System.Console.Write(String.Format(" {0,5:0.##} |", Elements[iterR, iterC]));
				}

				// Строка матрицы закончилась, переходим на следующую
				System.Console.Write('\n');
			}
			System.Console.WriteLine();
		}
	}

	/* Класс, описывающий адаптированную для условия текущей задачи матрицу,
	 * например квадратную или треугольную. По условию задач данный класс
	 * унаследован от обычной матрицы
	 */
	public class MatrixEx:MatrixBasic
	{
		// Конструктор по умолчанию
		public MatrixEx()
		{
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		/* Конструктор объекта
		 * В данном случае матрица состоит из двух столбцов - абсциссы и ординаты точки,
		 * при этом индекс строки соответствует "номеру точки - 1", т.к. считаем с нуля :)
		 */
		public MatrixEx(ushort rows)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = 2;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		// Относящиеся к задаче функции и вычисления

		// Проверяем, все ли точки плоскости лежат в кругу
		public bool IsInCircle(uint radius)
		{
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				double absc = this.Elements[iterR, 0];
				double ordn = this.Elements[iterR, 1];
				// Расстояние от нуля до точки поидее :)
				double rv = (double) Math.Abs(Math.Sqrt(Math.Pow(absc, 2) + Math.Pow(ordn, 2)));
				if(rv > radius)
					return false; // Одной точки, не лежащей в кругу, достаточно чтобы перестать считать
			}

			return true;
		}

		// Считаем и печатаем средние абсциссы и ординаты
		public void PrintWs()
		{
			double sumX = 0.00;
			double sumY = 0.00;

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				sumX += this.Elements[iterR, 0];
				sumY += this.Elements[iterR, 1];
			}

			// Средняя абсцисса
			double wX = sumX / this.Rows;
			// Средняя ордината
			double wY = sumY / this.Rows;

			System.Console.WriteLine(String.Format("Sredn. abscissa: {0,5:0.###}, ordinata: {1,5:0.###}", wX, wY));
		}

		// Распечатываем номера точек, не попавших в круг, аналогично проверке
		public void PrintExceptions(uint radius)
		{
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				double absc = this.Elements[iterR, 0];
				double ordn = this.Elements[iterR, 1];
				// Расстояние от нуля до точки поидее :)
				double rv = (double) Math.Abs(Math.Sqrt(Math.Pow(absc, 2) + Math.Pow(ordn, 2)));
				if(rv > radius)
				{
					// Точка не в кругу, распечатываем номер и расстояние до неё
					System.Console.WriteLine(String.Format("Tochka: {0} ne v krugu, abscissa: {1,5:0.###}, ordinata: {2,5:0.###}, rasst: {3,5:0.###}", (iterR + 1), absc, ordn, rv));
				}
			}
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			// Заданная по условию задачи величина (менять в коде, либо дописывать чтение ввода пользователя)
			uint excParam1 = Constants.PARAM1;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу
			MatrixEx me = new MatrixEx(Constants.TOTAL_POINTS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			if(me.IsInCircle(excParam1))
			{
				System.Console.WriteLine("Vse tochki v krugu");
				// Все точки плоскости попали в круг, печатаем средние координаты
				me.PrintWs();
			}
			else
			{
				System.Console.WriteLine("Ne vse tochki v krugu");
				// Не все точки плоскости попали в круг, печатаем номера исключений
				me.PrintExceptions(excParam1);
			}

			Console.ReadKey();
		}
	}
}

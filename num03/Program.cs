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
		public const ushort MAX_RND = 50;

		// Для задачи

		// Отрезок от PARAM1 до PARAM2, соответственно значение 1 должно быть задан меньше, чем 2
		public const double PARAM1 = -100;
		public const double PARAM2 = 100;
		public const ushort MATRIX_ROWS = 20;
		public const ushort MATRIX_COLS = 4;
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

		// Конструктор объекта
		public MatrixEx(ushort rows, ushort cols)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = (cols != 0) ? cols : (ushort) 2;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		// Относящиеся к задаче функции и вычисления

		// Проверить условие задачи
		public bool CheckCond(double[] sums, double p1, double p2)
		{
			bool flag = true;
			int iter = 0;

			foreach(double cval in sums)
			{
				if(cval < p1 || cval > p2)
				{
					System.Console.WriteLine(String.Format("V {0} stroke summa elementov {1,5:0.###}, ne popadaet v zadanny otrezok [{2}; {3}]", iter + 1, cval, p1, p2));
					flag = false;
				}
				iter++;
			}

			return flag; // true если все суммы эл-тов попадают на заданный участок, иначе false
		}

		// Определить номер строки с максимальной суммой элементов
		public void PrintMaxLineNumber(double[] sums)
		{
			double max = sums[0];
			ushort ind = 0;
			ushort iter = 0;

			foreach(double cval in sums)
			{
				if(cval >= max)
				{
					max = cval;
					ind = iter;
				}
				iter++;
			}

			System.Console.WriteLine(String.Format("Max summa elementov v {0} stroke: {1,5:0.###}", ind + 1, max));
		}

		// Посчитать суммы элементов для строк
		public double[] GetElementsSums()
		{
			/* Массив, размер которого равен кол-ву строк матрицы, каждой строке
			 * соответствует величина, равная сумме всех элементов этой строки
			 * матрицы... Не придумал как кошернее сделать %)
			 */
			double[] ret = new double[this.Rows];

			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				ret[iterR] = 0.00;
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					ret[iterR] += Elements[iterR, iterC];
				}
			}

			return ret;
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			// Заданные по условию задачи величины (менять в коде, либо дописывать чтение ввода пользователя)
			double excParam1 = Constants.PARAM1;
			double excParam2 = Constants.PARAM2;

			double[] sums;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			// Считаем суммы элементов по строкам матрицы и запоминаем результат - пригодится
			sums = me.GetElementsSums();

			if(me.CheckCond(sums, excParam1, excParam2))
			{
				// Все суммы элементов строк попадают на заданный отрезок
				System.Console.WriteLine("Vse summy elementov popadaut na zadanny otrezok");
				me.PrintMaxLineNumber(sums);
			}

			Console.ReadKey();
		}
	}
}

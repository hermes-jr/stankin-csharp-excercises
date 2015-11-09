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
		public const ushort MAX_RND = 5;

		// Для задачи
		public const ushort MATRIX_SIDE = 5;
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
		public int[,] Elements;

		// Индексатор, по условию задачи
		public int this[int row, int col]
		{
			get
			{
				return Elements[row, col];
			}
			set
			{
				Elements[row, col] = (int) value;
			}
		}

		// Конструктор класса по умолчанию
		public MatrixBasic()
		{
			this.Rows = 3;
			this.Columns = 2;
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixBasic(ushort rows, ushort cols)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = (cols != 0) ? cols : (ushort) 2;
			Elements = new int[this.Rows, this.Columns];
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
					System.Console.Write(String.Format(" {0,3} |", Elements[iterR, iterC]));
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
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixEx(ushort side)
		{
			this.Rows = (side != 0) ? side : (ushort) 2;
			this.Columns = this.Rows;
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Относящиеся к задаче функции и вычисления

		// Находим и возвращаем номер строки с максимальной суммой элементов
		public ushort GetMaxSumRow()
		{
			ushort ret = 0;
			double maxSum = 0.00;

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				double curSum = 0.00;
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					curSum += this.Elements[iterR, iterC];
				}
				if(curSum >= maxSum)
				{
					maxSum = curSum;
					ret = iterR;
				}
			}

			return ret;
		}

		// Находим и возвращаем номер столбца с максимальной суммой элементов
		public ushort GetMaxSumCol()
		{
			ushort ret = 0;
			double maxSum = 0.00;

			for(ushort iterC = 0; iterC < this.Columns; iterC++)
			{
				double curSum = 0.00;
				for(ushort iterR = 0; iterR < this.Rows; iterR++)
				{
					curSum += this.Elements[iterR, iterC];
				}
				if(curSum >= maxSum)
				{
					maxSum = curSum;
					ret = iterC;
				}
			}

			return ret;
		}

		// Находим и возвращаем сумму элементов столбца и строки с максимальными суммами элементов
		public double GetCrossSum(ushort col)
		{
			double ret = 0.00;

			ret += this.Elements[col, col]; // Один раз считаем элемент на пересечении столбца и строки
			// Потом прибавляем сумму остальных элементов столбца
			for(ushort iterC = 0; iterC < this.Columns; iterC++)
			{
				if(iterC != col)
					ret += this.Elements[col, iterC];
			}
			// ...и остальных элементов строки
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				if(iterR != col)
					ret += this.Elements[iterR, col];
			}

			return ret;
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			ushort maxSumRow;
			ushort maxSumCol;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём квадратную матрицу, в конструктор передаём размер матрицы
			MatrixEx me = new MatrixEx(Constants.MATRIX_SIDE);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();


			maxSumRow = me.GetMaxSumRow();
			maxSumCol = me.GetMaxSumCol();
			System.Console.WriteLine(String.Format("Maksimalnye summy elementov v {0} stroke i {1} stolbce", maxSumRow + 1, maxSumCol + 1));

			if(maxSumRow == maxSumCol)
				System.Console.WriteLine(String.Format("Summa elementov {0} stroki i {0} stolbca ravna {1,5:0.####}", maxSumRow + 1, me.GetCrossSum(maxSumRow)));

			Console.ReadKey();
		}
	}
}
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
		public const ushort PARAM1 = 3; // Параметр k по условию задачи
		public const ushort MATRIX_ROWS = 3;
		public const ushort MATRIX_COLS = 7;
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

		// "Служебная" функция, определение минимальной суммы среди столбцов для последующего сравнения
		private double GetMinSum()
		{
			double ret = 0.00;

			// Сначала считаем сумму нулевого (первого) столбца
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				ret += this.Elements[iterR, 0];
			}

			// Если в матрице только один столбец, валим из функции
			if(this.Columns == 1)
				return ret;

			/* Сравниваем суммы остальных столбцов и возвращаем минимальную из них.
			 * Так сделано на случай, если например во всех столбцах будет сумма -3, то есть
			 * для любого из столбцов будет верно отверждение, что сумма для них минимальна.
			 */
			for(ushort iterC = 1; iterC < this.Columns; iterC++)
			{
				double curSum = 0;
				for(ushort iterR = 0; iterR < this.Rows; iterR++)
				{
					curSum += this.Elements[iterR, iterC];
				}
				if(curSum < ret)
					ret = curSum;
			}

			return ret;
		}

		// Проверяем если в k столбце минимальная сумма элементов среди столбцов
		public bool IsMinSumIn(ushort k)
		{
			double minSum = GetMinSum();
			double curSum = 0;

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				curSum += this.Elements[iterR, k];
			}
		
			if(curSum == minSum)
				return true; // В случае, если в данном столбце сумма минимальна среди столбцов по матрице
			else
				return false; // Или не минимальна %)
		}

		public double GetColSum(ushort pFrom, ushort pTo)
		{
			double ret = 0.00;

			// Бежим либо от 0 столбца до заданного, либо от заданного до последнего включительно
			if(pTo == 0)
				pTo = (ushort) (this.Columns - 1);

			for(int iterC = pFrom; iterC <= pTo; iterC++)
			{
				for(int iterR = 0; iterR < this.Rows; iterR++)
				{
					ret += this.Elements[iterR, iterC];
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
			// Заданная по условию задачи величина (менять в коде, либо дописывать чтение ввода пользователя)
			ushort excParam1 = Constants.PARAM1;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			if(me.IsMinSumIn((ushort) (excParam1 - 1)))
			{
				System.Console.WriteLine(String.Format("Summa v {0} stolbce minimalna", excParam1));
				// В k-ом столбце минимальная сумма элементов, ищем сумму до k столбца
				System.Console.WriteLine(String.Format("Summa do {0} stolbca ravna: {1,5:0.##}", excParam1, me.GetColSum(0, (ushort) (excParam1 - 1))));
			}
			else
			{
				System.Console.WriteLine(String.Format("Summa v {0} stolbce ne minimalna", excParam1));
				// Не максимальная сумма, ищем сумму после k
				System.Console.WriteLine(String.Format("Summa posle {0} stolbca ravna: {1,5:0.##}", excParam1, me.GetColSum((ushort)(excParam1 - 1), 0)));
			}

			Console.ReadKey();
		}
	}
}

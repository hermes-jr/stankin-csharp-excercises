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

		public const ushort MATRIX_ROWS = 6;
		public const ushort MATRIX_COLS = 3;
	}

	/* Класс в котором реализована функция определения простых чисел, странно что
	 * в стандартном Math этого нет. Если есть - обязательно заменить в коде на встроенные функции
	 */
	class MathPrimes
	{
		public static bool IsPrime(int number)
		{
			int iter;
		
			for(iter = 2; iter < number; iter++)
			{
				if(number % iter == 0)
					return false; // Не простое число
			}
			if(iter == number)
				return true; // Простое число

			return false; // Не должно быть
		}
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
		protected int TotalElements { get; set; }

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
			TotalElements = 0;
			Random random = new Random();
			/* Бежим по матрице слева направо сверху вниз (большинство циклов
			 * будут реализованы так)
			 */
			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					/* Впихнул сюда чтобы в каждом конструкторе не писать TotalElements = Rows * Columns. Это
					 * крайне неэффективно и индус-стайл, зато в одном месте :) Если не лень - пихай куда положено
					 */
					TotalElements++;

					// ~случайный выбор значения элемента и тут же присвоение знака
					int currentNumber = random.Next(Constants.MAX_RND);
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
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Конструктор объекта
		public MatrixEx(ushort rows, ushort cols)
		{
			this.Rows = (rows != 0) ? rows : (ushort) 2;
			this.Columns = (cols != 0) ? cols : (ushort) 2;
			Elements = new int[this.Rows, this.Columns];
			this.Populate();
		}

		// Относящиеся к задаче функции и вычисления

		// Вычисляем условие задачи если больше половины элементов простые числа
		public bool IsMoreThanHalfPrimes()
		{
			int numPrimes = 0;

			for(int iterR = 0; iterR < this.Rows; iterR++)
			{
				for(int iterC = 0; iterC < this.Columns; iterC++)
				{
					// Если число простое, считаем его
					if(MathPrimes.IsPrime(this.Elements[iterR, iterC]))
						numPrimes++;
					// Если перевалили за половину, перестаём считать
					if(((double) numPrimes) / ((double) this.TotalElements) >= 0.5)
						return true;
				}
			}

			return false;
		}

		// Сортировка последнего столбца по убыванию пузырьком
		public void SortLastCol()
		{
			for(int iterR = this.Rows - 1; iterR > 0 ; iterR--)
			{
				for(int iterI = 0; iterI < iterR ; iterI++)
				{
					if(this.Elements[iterI, this.Columns - 1] < this.Elements[iterI + 1, this.Columns - 1])
					{
						int tmp = this.Elements[iterI, this.Columns - 1];
						this.Elements[iterI, this.Columns - 1] = this.Elements[iterI + 1, this.Columns - 1];
						this.Elements[iterI + 1, this.Columns - 1] = tmp;
					}
				}
			}
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			if(me.IsMoreThanHalfPrimes())
			{
				System.Console.WriteLine("V massive bolshe poloviny prostyh chisel");
				me.SortLastCol();
				me.Print();
			}
			else
			{
				System.Console.WriteLine("V massive menshe poloviny prostyh chisel");
			}

			Console.ReadKey();
		}
	}
}

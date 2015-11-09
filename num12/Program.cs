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
				Elements[row, col] = (int) value;
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
					System.Console.Write(String.Format(" {0,5:0.###} |", Elements[iterR, iterC]));
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
		public MatrixEx(ushort side)
		{
			this.Rows = (side != 0) ? side : (ushort) 2;
			this.Columns = this.Rows;
			Elements = new double[this.Rows, this.Columns];
			this.Populate();
		}

		// Относящиеся к задаче функции и вычисления

		// Подсчёт среднего арифметического элементов выше главной диагонали
		public double CalcUpperW()
		{
			ushort iter = 1;
			double tot = 0.00;
			double sum = 0.00;

			// Бежим по матрице, Rows = Cols, но тут для наглядности разные переменные
			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = iter; iterC < this.Columns; iterC++)
				{
					sum += this.Elements[iterR, iterC];
					tot++;
				}
				iter++;
			}

			// По идее в матрице не может быть 0 элементов, но на всякий случай проверка, ибо на ноль делить нельзя :)
			if(tot != 0)
				return sum/tot;
			else
				return 0.00;
		}

		// Ищем индекс строки с минимальной суммой элементов
		public ushort getMinSumLineId()
		{
			double minVal = 0.00;
			ushort ret = 0;

			// Сначала считаем сумму нулевой (первой) строки
			for(ushort iterC = 0; iterC < this.Columns; iterC++)
			{
				minVal += this.Elements[0, iterC];
			}

			// Если в матрице только одна строка, валим из функции
			if(this.Rows == 1)
				return ret;

			// Сравниваем суммы остальных строк и возвращаем индекс минимальной из них
			for(ushort iterR = 1; iterR < this.Rows; iterR++)
			{
				double curSum = 0;
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					curSum += this.Elements[iterR, iterC];
				}
				if(curSum < minVal)
				{
					minVal = curSum;
					ret = iterR;
				}
			}

			return ret;
		}

		// Умножаем как сказано в условии
		public void Multiply(ushort row, double multiplier)
		{
			for(ushort iterC = 0; iterC < this.Columns; iterC++)
			{
				this.Elements[row, iterC] *= multiplier;
			}
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			double upperW;
			ushort minSumLineId;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём квадратную матрицу, в конструктор передаём размер матрицы
			MatrixEx me = new MatrixEx(Constants.MATRIX_SIDE);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();

			upperW = me.CalcUpperW();
			System.Console.WriteLine(String.Format("Srednee arifmeticheskoe el-tov vyshe glavnoy diagonali {0,5:0.###}", upperW));

			minSumLineId = me.getMinSumLineId();
			System.Console.WriteLine(String.Format("V {0} stroke minimalnaya summa elementov", minSumLineId + 1));

			// Умножаем элементы строки minSumLineId на среднее арифметическое элементов выше диагонали upperW
			me.Multiply(minSumLineId, upperW);
			me.Print();

			Console.ReadKey();
		}
	}
}

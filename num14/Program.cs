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
		public const ushort MATRIX_ROWS = 5;
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

		// Функция возвращает индекс столбца с наибольшим количеством отрицательных элементов
		public ushort WhichIsMaxNegative()
		{
			ushort ret = 0;
			int maxNegatives = 0;
			int curNegatives;

			for(ushort iterC = 0; iterC < this.Columns; iterC++)
			{
				curNegatives = 0;
				for(ushort iterR = 0; iterR < this.Rows; iterR++)
				{
					if(this.Elements[iterR, iterC] < 0)
						curNegatives++;
				}
				// По столбцу пробежали, сравниваем:
				if(curNegatives > maxNegatives)
				{
					maxNegatives = curNegatives;
					ret = iterC;
				}
			}

			return ret;
		}

		/* По условию задачи меняем отрицательные элементы (тут для простоты - вообще все) для столбцов
		 * больше, чем заданный, на их модули
		 */
		public void AbsoluteAfter(ushort after)
		{
			for(ushort iterC = after; iterC < this.Columns; iterC++)
			{
				for(ushort iterR = 0; iterR < this.Rows; iterR++)
				{
					this.Elements[iterR, iterC] = Math.Abs(this.Elements[iterR, iterC]);
				}
			}
		}

		// Считаем среднее арифметическое матрицы, всё тупо
		public double CalcW()
		{
			double sum = 0.00;
			double tot = 0.00;

			for(ushort iterR = 0; iterR < this.Rows; iterR++)
			{
				for(ushort iterC = 0; iterC < this.Columns; iterC++)
				{
					sum += this.Elements[iterR, iterC];
					tot++;
				}
			}

			// По идее в матрице не может быть 0 элементов, но на всякий случай проверка, ибо на ноль делить нельзя :)
			if(tot != 0)
				return sum/tot;
			else
				return 0.00;
		}
	}

	// Собственно класс программы, тут всё тупо
	class Program
	{
		static void Main(string[] args)
		{
			// Среднее арифметическое матрицы для сравнения
			double oldW = 0;
			ushort mneg;

			// Чистим чтобы старое не мешалось :)
			Console.Clear();

			// Создаём прямоугольную матрицу, в конструктор передаём размер матрицы
			MatrixEx me = new MatrixEx(Constants.MATRIX_ROWS, Constants.MATRIX_COLS);
			// Выводим таблицу, метод Print описан в классе базовой матрицы и унаследован
			me.Print();
			// Выводим среднее арифметическое изначально сгенерированной матрицы
			oldW = me.CalcW();
			System.Console.WriteLine(String.Format("Sredneye arifmet: {0,6:0.####}", oldW));

			// Определяем столбец в котором максимальное количество отрицательных элементов
			mneg = me.WhichIsMaxNegative();
			System.Console.WriteLine(String.Format("V {0} stolbce bolshe vsego otricatelnyh elementov", mneg + 1));

			me.AbsoluteAfter(mneg);
			me.Print();

			// Отвечаем на вопрос о том, как изменилось среднее арифметическое
			double newW = me.CalcW();
			if(newW == oldW)
				System.Console.WriteLine(String.Format("Srednee arifmeticheskoe ne izmenilos': {0,6:0.####}", newW));
			if(newW < oldW)
				System.Console.WriteLine(String.Format("Srednee arifmeticheskoe umenshilos': {0,6:0.####}", newW));
			if(newW > oldW)
				System.Console.WriteLine(String.Format("Srednee arifmeticheskoe vozroslo: {0,6:0.####}", newW));

			Console.ReadKey(); // Ждём выброс и валим нахер!
		}
	}
}

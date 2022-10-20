using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


class Játék
{
	class PontszamFigyeles
	{
		private int felsoHatar;
		private int MaxPont;

		public PontszamFigyeles(int ElertFelsoHatar)
		{
			felsoHatar = ElertFelsoHatar;
		}

		public void Add(int x)
		{
			MaxPont = x;
			if (MaxPont >= felsoHatar)
			{
				Elérte(EventArgs.Empty);
			}
		}

		protected virtual void Elérte(EventArgs e)
		{
			EventHandler handler = Küszöb_elérve;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public event EventHandler Küszöb_elérve;
	}

	private string jatekNeve;
	private bool folyamatban;
	private int maximalisPontszam;
	private int jatekosokSzama;
	Dictionary<int, int> jatekosokPontszama = new Dictionary<int, int>();

	public Játék()
	{
	}

	public Játék(int maximalisPontszam, int jatekosokSzama)
	{
		this.maximalisPontszam = maximalisPontszam;
		this.jatekosokSzama = jatekosokSzama;
		for (int i = 0; i < this.jatekosokSzama; i++)
		{
			jatekosokPontszama.Add(i, 0);
		}
	}

	public Játék(bool folyamatban, int maximalisPontszam, int jatekosokSzama)
	{
		this.folyamatban = folyamatban;
		this.maximalisPontszam = maximalisPontszam;
		this.jatekosokSzama = jatekosokSzama;
		for (int i = 0; i < this.jatekosokSzama; i++)
		{
			jatekosokPontszama.Add(i, 0);
		}
	}

	public Játék(string jatekNeve, bool folyamatban, int maximalisPontszam, int jatekosokSzama)
	{
		this.jatekNeve = jatekNeve;
		this.folyamatban = folyamatban;
		this.maximalisPontszam = maximalisPontszam;
		this.jatekosokSzama = jatekosokSzama;
		for (int i = 0; i < this.jatekosokSzama; i++)
		{
			jatekosokPontszama.Add(i, 0);
		}
	}

	public string JatekNeve
	{
		get => jatekNeve;
		set => jatekNeve = value;
	}

	public bool Folyamatban
	{
		get => folyamatban;
		set => folyamatban = value;
	}

	public int MaximalisPontszam
	{
		get => maximalisPontszam;
		set => maximalisPontszam = value;
	}

	public int JatekosokSzama
	{
		get => jatekosokSzama;
		set => jatekosokSzama = value;
	}

	public Dictionary<int, int> JatekosokPontszama
	{
		get => jatekosokPontszama;
		set => jatekosokPontszama = value;
	}

	public void PontSzerzés(int jatekosSzama, int pontszam)
	{
		if (jatekosokPontszama.ContainsKey(jatekosSzama))
		{
			jatekosokPontszama[jatekosSzama] += pontszam;
		}

		PontszamFigyeles pontszamFigyeles = new PontszamFigyeles(maximalisPontszam);
		pontszamFigyeles.Küszöb_elérve += pontszamElerve;
		pontszamFigyeles.Add(MaximalisPontszamuJatekos().Item2);
	}



	public Tuple<int, int> MaximalisPontszamuJatekos()
	{
		int jatekosSzama = 0;
		int maxPontszam = jatekosokPontszama[0];
		foreach (var pont in jatekosokPontszama)
		{
			if (maxPontszam < pont.Value)
			{
				jatekosSzama = pont.Key;
				maxPontszam = pont.Value;
			}
		}

		return new Tuple<int, int>(jatekosSzama, maxPontszam); ;
	}

	void pontszamElerve(object sender, EventArgs e)
	{
		var jatekos = MaximalisPontszamuJatekos();
		Console.WriteLine("\nPontszam elerve!" + " Játékos: " + jatekos.Item1 + " Pontszám: " + jatekos.Item2);
	}

	public int OsszPontszam()
	{
		int sum = 0;
		foreach (var pontszam in jatekosokPontszama)
		{
			sum += pontszam.Value;
		}

		return sum;
	}


}



internal class Program
{
	public static void ToBinaryFile(string? path, string data)
	{
		path ??= "../../../games.dat";
		using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
		{
			writer.Write(data + "\n");
		}
	}

	public static void Main(string[] args)
	{
		var jatekok = new Dictionary<string, Játék>();
		Játék jatek1 = new Játék();

		for (int i = 0; i < 5; i++)
		{
			Random random = new Random();
			int jatekosokSzama = random.Next(2, 10);
			bool folyamatban = (random.Next(50) <= 25) ? true : false;
			Játék játék = new Játék(i.ToString(), folyamatban, random.Next(10, 50), jatekosokSzama);
			játék.PontSzerzés(random.Next(1, jatekosokSzama - 1), random.Next(1, 100));
			jatekok.Add(i.ToString(), játék);
		}

		foreach (var jatek in jatekok)
		{
			Console.WriteLine("Jatek neve: " + jatek.Key + ", Jatekosok szama: " + jatek.Value.JatekosokSzama +
							  ", Osszpontszam: " + jatek.Value.OsszPontszam());
		}



		var atlagJatekosszam = jatekok.Values.Average(x => x.JatekosokSzama);
		Console.WriteLine("\nAtlag jatekosszam> " + atlagJatekosszam);

		var folyamatbanLevoJatekokPontszamai = jatekok.Values.Where(x => x.Folyamatban).ToList();
		Console.WriteLine("\nA folyamatban lévő lévő játékok maximális pontszámai:");

		string data = "";
		foreach (var jatek in jatekok)
		{
			data += jatek.Value.JatekNeve + " " + jatek.Value.JatekosokPontszama + " " + jatek.Value.JatekosokSzama + " " +
					jatek.Value.MaximalisPontszam + " " + jatek.Value.Folyamatban + "\n";
			Console.WriteLine("Sikeres mentés");
		}
		ToBinaryFile("jatek.dat", data);
	}
}


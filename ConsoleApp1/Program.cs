using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {
            while(true) {
                Console.WriteLine("Please Select an Option:\n" + "" +
                                  "\t1) Add a new word and defintion\n" + "" +
                                  "\t2) Delete an existing word\n" + "" +
                                  "\t3) Play the game\n" + "" +
                                  "\t4) exit\n" +
                                  "\t5) View the list of words\n" + 
                                  "\t6) Clear the database.\n" + 
                                  "\t7) Write current words to a file.");
                try {
                    using (var db = new WordContext()) {
                        int UserInput = Convert.ToInt16(Console.ReadLine());
                        switch (UserInput) {
                            case 1:
                                Console.WriteLine("Enter the new word to be added:");
                                String NewWord = Console.ReadLine();
                                if(db.Words.ToList().Find(c => c.WordId.Equals(NewWord)) != null) {
                                    Console.WriteLine("Word is already contained.");
                                    break;
                                }
                                if (NewWord.Contains(' ') || NewWord == null) throw new FormatException();
                                Console.WriteLine("Enter the new word's definition:");
                                String Definition = Console.ReadLine();
                                if (Definition == null) throw new FormatException();
                                db.Words.Add(new Word(NewWord, Definition));
                                db.SaveChanges();
                                break;
                            case 2:
                                if(db.Words.Count() == 0) {
                                    Console.WriteLine("There are currently no words");
                                    break;
                                }
                                Console.WriteLine("Enter the word to be deleted:");
                                String WordToDelete = Console.ReadLine();
                                Word entry = db.Words.ToList().Find(c => c.WordId.Equals(WordToDelete));
                                if(WordToDelete == null || entry == null) {
                                    Console.WriteLine("Word not found.");
                                }
                                db.Words.Remove(entry);
                                db.SaveChanges();
                                break;
                            case 3:
                                int count = db.Words.Count();
                                if(count == 0) {
                                    Console.WriteLine("Please add at least one word to play the game.");
                                    break;
                                }
                                int correct = 0;
                                foreach(Word w in db.Words.ToList()) {
                                    Console.WriteLine("What is the definition for the word {0}", w.WordId);
                                    String UserAnswer = Console.ReadLine();
                                    if(UserAnswer.Equals(w.Defintion)) {
                                        Console.WriteLine("Correct!");
                                        ++correct;                                    
                                    }
                                    else {
                                        Console.WriteLine("Sorry, that is incorrect");
                                    }
                                }
                                Console.WriteLine("Game over!\nYou got {0} out of {1} word defintions right!", correct, count);
                                break;
                            case 4:
                                Environment.Exit(0);
                                break;
                            case 5:
                                if(db.Words.Count() == 0) {
                                    Console.WriteLine("There are currently no words.");
                                    break;
                                }
                                Console.WriteLine("The current words are:");
                                foreach(Word w in db.Words.ToList()) {
                                    Console.WriteLine("\t{0}: {1}", w.WordId, w.Defintion);
                                }
                                break;
                            case 6:
                                db.Words.RemoveRange(db.Words);
                                db.SaveChanges();
                                break;
                            case 7:
                                Console.WriteLine("Provide a file name:");
                                String FileName = Console.ReadLine();
                                if (File.Exists(FileName)) File.Delete(FileName);
                                FileStream fs = File.Create(FileName);
                                db.Words.ForEachAsync(w => File.WriteAllLines(FileName, new String[] {w.WordId + ": " + w.Defintion}));
                                fs.Flush();
                                break;
                            default: throw new FormatException();
                        }
                    }
                } catch(FormatException) {
                    Console.WriteLine("Invalid User Input. Please try again.");
                    continue;
                }
            }
        }
    }

    public class Word {
        public Word() {
            WordId = null;
            Defintion = null;
        }
        public Word(String word, String definition) {
            WordId = word;
            Defintion = definition;
        }
        public String WordId { get; set; }
        public String Defintion { get; set; }
    }

    public class WordContext : DbContext {
        public DbSet<Word> Words { get; set; }
    }
}

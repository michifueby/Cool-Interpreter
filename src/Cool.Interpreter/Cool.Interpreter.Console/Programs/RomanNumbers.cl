(* Roman Numeral Converter *)
(* Converts decimal numbers to Roman numerals *)

class Main inherits IO {
    main(): Int {{
        let converter: RomanConverter <- new RomanConverter in
        converter.convert(2026);  -- Example: convert 2026 to MMXXVI
        0;
    }};
};

class RomanConverter inherits IO {
    (* Converts and prints a number in Roman numeral format *)
    convert(n: Int): Object {{
        out_int(n);
        out_string(" in Roman: ");
        out_string(to_roman(n));
        out_string("\n");
    }};

    (* Converts an integer to Roman numeral string *)
    (* Uses greedy algorithm: subtract largest values first *)
    to_roman(num: Int): String {
        let result: String <- "",
            n: Int <- num in {
        result <- add_roman(result, n, 1000, "M");
        n <- n - (n / 1000 * 1000);
        result <- add_roman(result, n, 900, "CM");
        n <- n - (n / 900 * 900);
        result <- add_roman(result, n, 500, "D");
        n <- n - (n / 500 * 500);
        result <- add_roman(result, n, 400, "CD");
        n <- n - (n / 400 * 400);
        result <- add_roman(result, n, 100, "C");
        n <- n - (n / 100 * 100);
        result <- add_roman(result, n, 90, "XC");
        n <- n - (n / 90 * 90);
        result <- add_roman(result, n, 50, "L");
        n <- n - (n / 50 * 50);
        result <- add_roman(result, n, 40, "XL");
        n <- n - (n / 40 * 40);
        result <- add_roman(result, n, 10, "X");
        n <- n - (n / 10 * 10);
        result <- add_roman(result, n, 9, "IX");
        n <- n - (n / 9 * 9);
        result <- add_roman(result, n, 5, "V");
        n <- n - (n / 5 * 5);
        result <- add_roman(result, n, 4, "IV");
        n <- n - (n / 4 * 4);
        result <- add_roman(result, n, 1, "I");
        result;
        }
    };

    (* Recursively adds Roman numerals to the result string *)
    (* Repeats the symbol while value fits into remaining number *)
    add_roman(current: String, n: Int, value: Int, roman: String): String {
        if value < n + 1 then  -- If value fits into n
            add_roman(current.concat(roman), n - value, value, roman)  -- Add symbol and recurse
        else
            current  -- Return accumulated result
        fi
    };
};

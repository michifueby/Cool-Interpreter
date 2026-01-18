(* Pascal's Triangle Generator *)
(* Generates and prints the first 15 rows of Pascal's triangle *)

class Main {
    main(): Int {
        {
        let row: Int <- 0 in
        while row < 15 loop
            {
            let col: Int <- 0 in
            (* Print each element in the current row *)
            while col <= row loop {
                (new IO).out_int(pascal(row, col)).out_string(" ");
                col <- col + 1;
            }
            pool;
            (new IO).out_string("\n");
            row <- row + 1;
            }
        pool;
        0;
        }
    };

    (* Calculates Pascal's triangle value at position (n, k) *)
    (* Uses recursion: C(n,k) = C(n-1,k-1) + C(n-1,k) *)
    pascal(n: Int, k: Int): Int {
        if k = 0 then 1                              -- Edge: first element
        else if k = n then 1                          -- Edge: last element
        else pascal(n - 1, k - 1) + pascal(n - 1, k) fi fi  -- Sum of two above
    };
};

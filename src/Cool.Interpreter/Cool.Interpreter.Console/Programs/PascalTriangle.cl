class Main {
    main(): Int {
        {
        let row: Int <- 0 in
        while row < 15 loop
            {
            let col: Int <- 0 in
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

    pascal(n: Int, k: Int): Int {
        if k = 0 then 1
        else if k = n then 1
        else pascal(n - 1, k - 1) + pascal(n - 1, k) fi fi
    };
};

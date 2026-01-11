class Main {
    main(): Int {
        {
        let i: Int <- 2 in
        while i < 100 loop {
            if is_prime(i) then
                (new IO).out_int(i).out_string(" ")
            else 0 fi;
            i <- i + 1;
        }
        pool;
        (new IO).out_string("\n");
        0;
        }
    };

    is_prime(n: Int): Bool {
        {
        let i: Int <- 2,
            result: Bool <- true in
        {
        while i * i <= n loop
            {
            if n - (n / i * i) = 0 then {
                result <- false;
                i <- n;
            } else 0 fi;
            i <- i + 1;
            }
        pool;
        result;
        };
        }
    };
};

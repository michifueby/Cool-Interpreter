(* Prime Numbers Generator *)
(* Finds and prints all prime numbers less than 100 *)

class Main {
    main(): Int {
        {
        let i: Int <- 2 in  -- Start from 2 (first prime)
        while i < 100 loop {
            if is_prime(i) then
                (new IO).out_int(i).out_string(" ")  -- Print prime number
            else 0 fi;
            i <- i + 1;
        }
        pool;
        (new IO).out_string("\n");
        0;
        }
    };

    (* Checks if a number is prime using trial division *)
    (* Tests divisibility up to square root of n *)
    is_prime(n: Int): Bool {
        {
        let i: Int <- 2,
            result: Bool <- true in
        {
        while i * i <= n loop  -- Only check up to sqrt(n)
            {
            if n - (n / i * i) = 0 then {  -- Check if n is divisible by i
                result <- false;            -- Not prime
                i <- n;                     -- Exit loop early
            } else 0 fi;
            i <- i + 1;
            }
        pool;
        result;
        };
        }
    };
};

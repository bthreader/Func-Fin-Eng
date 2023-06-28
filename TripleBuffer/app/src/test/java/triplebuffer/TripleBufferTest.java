package triplebuffer;

import static org.junit.jupiter.api.Assertions.assertEquals;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

public class TripleBufferTest {
    private TripleBuffer<Integer> tripleBuffer;

    @BeforeEach
    void setup() {
        tripleBuffer = new TripleBuffer<>(1, 2, 3);
    }

    @Test
    void testSingleWriteAndRead() {
        tripleBuffer.write(50);
        assertEquals(50, tripleBuffer.read());
    }

    @Test
    void multipleWritesAndOneRead() {
        tripleBuffer.write(50);
        tripleBuffer.write(10);
        tripleBuffer.write(22);
        assertEquals(22, tripleBuffer.read());
    }

    // @Test
    // void testWriteAndReadConcurrent() throws InterruptedException {
    // Thread writerThread = new Thread(() -> {
    // for (int i = 1; i <= 10; i++) {
    // tripleBuffer.write(i);
    // }
    // });

    // Thread readerThread = new Thread(() -> {
    // for (int i = 1; i <= 10; i++) {
    // int value = tripleBuffer.read();
    // assertEquals(i, value);
    // }
    // });

    // // Start the writer and reader threads
    // writerThread.start();
    // readerThread.start();

    // // Wait for both threads to complete
    // writerThread.join();
    // readerThread.join();
    // }
}

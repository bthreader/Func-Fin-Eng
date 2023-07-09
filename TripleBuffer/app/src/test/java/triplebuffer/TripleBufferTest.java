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
    void initalizesProperly() {
        // Clean buffer is set to unread, which means that read will swap the
        // initial output buffer (3) with the clean buffer (2)
        int value = tripleBuffer.read();

        assertEquals(2, value);
    }

    @Test
    void singleWriteAndRead() {
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

    @Test
    void oneWriteAndMultipleReads() {
        tripleBuffer.write(50);
        assertEquals(50, tripleBuffer.read());
        assertEquals(50, tripleBuffer.read());
        assertEquals(50, tripleBuffer.read());
    }
}

package triplebuffer.buffer;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;
import org.junit.jupiter.api.BeforeEach;

class SharedStateTest {
    private SharedState<Integer> sharedState;
    private final int initialInputIndex = 0;
    private final int initialOutputIndex = 2;

    @BeforeEach
    void setup() {
        sharedState = new SharedState<>(1, 2, 3);
    }

    @Test
    void initalizesProperly() {
        assertTrue(sharedState.cleanIsUnread());
        assertEquals(1, sharedState.getBuffer(0));
        assertEquals(2, sharedState.getBuffer(1));
        assertEquals(3, sharedState.getBuffer(2));
    }

    @Test
    void setBuffer() {
        // Sanity check - buffer 0 used to have value 1
        assertEquals(1, sharedState.getBuffer(0));

        // Act
        sharedState.setBuffer(0, 10);

        // Now it's 10
        assertEquals(10, sharedState.getBuffer(0));
    }

    @Test
    void swapOutputBuffer() {
        // Act
        int newOutputBufferIndex = sharedState.swapOutputBuffer(initialOutputIndex);

        // Assert
        assertEquals(1, newOutputBufferIndex);
        assertEquals(2, sharedState.getBuffer(newOutputBufferIndex));
        assertFalse(sharedState.cleanIsUnread());
    }

    @Test
    void swapInputBuffer() {
        // Arrange
        // Make cleanIsUnread = false via output buffer swap
        // As a consequence, back buffer now has index = 2
        sharedState.swapOutputBuffer(initialOutputIndex);

        // Act
        int newInputIndex = sharedState.swapBackBuffer(initialInputIndex);

        // Assert
        assertEquals(2, newInputIndex);
        assertEquals(3, sharedState.getBuffer(newInputIndex));
        assertTrue(sharedState.cleanIsUnread());
    }
}

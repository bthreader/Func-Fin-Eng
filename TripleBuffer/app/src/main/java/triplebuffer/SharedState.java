package triplebuffer;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.atomic.AtomicInteger;
import com.google.common.annotations.VisibleForTesting;

public class SharedState<T> {
    /**
     * 3-bit bit field.
     *
     * First bit is a flag which indicates whether there is a value in the clean backbuffer that
     * hasn't been read; `cleanIsUnread`.
     *
     * Second and third bit provide the index of the current clean backbuffer (0, 1, or 2).
     */
    private AtomicInteger backBufferState;
    private static final int CLEAN_INDEX_MASK = 0b11;
    private static final int CLEAN_IS_UNREAD = 0b100;

    private volatile List<T> buffers;

    /**
     * Creates a three buffer array and manages it's state.
     *
     * @param inputValue initial input buffer value: buffers[0]
     * @param cleanValue initial clean buffer value: buffers[1]
     * @param outputValue initial output buffer value: buffers[2]
     */
    public SharedState(T inputValue, T cleanValue, T outputValue) {
        buffers = new ArrayList<>(List.of(inputValue, cleanValue, outputValue));
        // Set clean backbuffer as unread, with index 1
        backBufferState = new AtomicInteger(0b101);
    }

    public T getBuffer(int index) {
        return buffers.get(index);
    }

    public void setBuffer(int index, T value) {
        buffers.set(index, value);
    }

    /**
     * Indicates whether there is a new value which hasn't been read yet.
     */
    @VisibleForTesting
    public boolean cleanIsUnread() {
        return ((backBufferState.get() & CLEAN_IS_UNREAD) != 0);
    }

    /**
     * Swaps the input buffer with the other (clean) backbuffer. Sets cleanIsUnread to true.
     *
     * @param inputIndex the index of the current input buffer.
     * @return the index of the new input buffer.
     */
    public int swapBackBuffer(int inputIndex) {
        int oldSharedState = backBufferState.getAndSet(inputIndex | CLEAN_IS_UNREAD);
        // Old clean backbuffer is the new input buffer
        return (oldSharedState & CLEAN_INDEX_MASK);
    }

    /**
     * Swaps the output buffer with the clean backbuffer. Sets cleanIsUnread to false.
     *
     * @param outputIndex
     * @return the index of the new output buffer.
     */
    public int swapOutputBuffer(int outputIndex) {
        int oldSharedState = backBufferState.getAndSet(outputIndex);
        // Old clean backbuffer is the new output buffer
        return (oldSharedState & CLEAN_INDEX_MASK);
    }
}

package triplebuffer.buffer;

import java.util.concurrent.atomic.AtomicInteger;

class SharedState<T> {
    /**
     * 3-bit bit field.
     *
     * First bit is a flag which indicates whether there is a value in the clean backbuffer that
     * hasn't been read; `cleanIsUnread`.
     *
     * Second and third bit together provide the index of the current clean backbuffer (0, 1, or 2).
     */
    private AtomicInteger backBufferState;
    private static final int CLEAN_INDEX_MASK = 0b11;
    private static final int CLEAN_IS_UNREAD = 0b100;

    private volatile T buffer0;
    private volatile T buffer1;
    private volatile T buffer2;

    public SharedState(T inputValue, T cleanValue, T outputValue) {
        // buffer0 is the initial input buffer
        buffer0 = inputValue;

        // buffer1 is the initial clean buffer
        buffer1 = cleanValue;

        // buffer2 is the initial output buffer
        buffer2 = outputValue;

        // Set clean backbuffer as unread, with index 1
        backBufferState = new AtomicInteger(0b101);
    }

    public T getBuffer(int index) {
        switch (index) {
            case 0: {
                return buffer0;
            }
            case 1: {
                return buffer1;
            }
            case 2: {
                return buffer2;
            }
            default: {
                throw new IndexOutOfBoundsException(index);
            }
        }
    }

    public void setBuffer(int index, T value) {
        switch (index) {
            case 0: {
                buffer0 = value;
                break;
            }
            case 1: {
                buffer1 = value;
                break;
            }
            case 2: {
                buffer2 = value;
                break;
            }
            default: {
                throw new IndexOutOfBoundsException(index);
            }
        }
    }

    /**
     * Indicates whether there is a new value which hasn't been read yet.
     */
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

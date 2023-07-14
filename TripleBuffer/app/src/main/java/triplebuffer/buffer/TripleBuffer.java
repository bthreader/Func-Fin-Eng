package triplebuffer.buffer;

import javax.annotation.concurrent.ThreadSafe;

@ThreadSafe
public class TripleBuffer<T> implements IProducer<T>, IConsumer<T> {
    private Integer inputIndex = 0;
    private Integer outputIndex = 2;
    private SharedState<T> sharedState;

    public TripleBuffer(T inputValue, T cleanValue, T outputValue) {
        sharedState = new SharedState<T>(inputValue, cleanValue, outputValue);
    }

    /**
     * Set the value of the input buffer.
     */
    public synchronized void write(T value) {
        sharedState.setBuffer(inputIndex, value);
        inputIndex = sharedState.swapBackBuffer(inputIndex);
    }

    /**
     * Read the value of the output buffer.
     */
    public synchronized T read() {
        if (sharedState.cleanIsUnread()) {
            // Swap the current output buffer with the clean back buffer
            outputIndex = sharedState.swapOutputBuffer(outputIndex);
        }

        return sharedState.getBuffer(outputIndex);
    }
}

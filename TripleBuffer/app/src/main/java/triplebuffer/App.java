package triplebuffer;

import java.util.Random;

import triplebuffer.buffer.IConsumer;
import triplebuffer.buffer.IProducer;
import triplebuffer.buffer.TripleBuffer;

public class App {
    public static void main(String[] args) {
        TripleBuffer<Double> marketABuffer = new TripleBuffer<>(1.28, 1.28, 1.28);
        IConsumer<Double> marketAConsumer = marketABuffer;
        IProducer<Double> marketAProducer = marketABuffer;

        TripleBuffer<Double> marketBBuffer = new TripleBuffer<>(1.28, 1.28, 1.28);
        IConsumer<Double> marketBConsumer = marketBBuffer;
        IProducer<Double> marketBProducer = marketBBuffer;

        Random random = new Random(0);

        Thread marketAProducerThread = new Thread(() -> {
            while (true) {
                boolean up = random.nextBoolean();
                if (up) {
                    marketAProducer.write(1.29 + random.nextDouble() * 0.001);
                } else {
                    marketAProducer.write(1.27 + random.nextDouble() * 0.001);
                }

                // Avoid tight looping
                try {
                    Thread.sleep(1);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });

        Thread marketBProducerThread = new Thread(() -> {
            while (true) {
                boolean up = random.nextBoolean();
                if (up) {
                    marketBProducer.write(1.30 + random.nextDouble() * 0.001);
                } else {
                    marketBProducer.write(1.26 + random.nextDouble() * 0.001);
                }

                // Avoid tight looping
                try {
                    Thread.sleep(1);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });

        Thread consumerThread = new Thread(() -> {
            while (true) {
                double gpbUsdA = marketAConsumer.read();
                double gbpUsdB = marketBConsumer.read();

                System.out.printf("%.4f", (gpbUsdA + gbpUsdB) / 2);
                System.out.println();

                // Avoid tight looping
                try {
                    Thread.sleep(1);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });

        // Start the writer and reader threads
        marketAProducerThread.start();
        marketBProducerThread.start();
        consumerThread.start();
    }
}

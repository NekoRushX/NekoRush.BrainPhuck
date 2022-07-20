## NekoRush.BrainPhuck
This is a brainf*** **emulator**  written in C#.  
With some advanced custom instructions supported.

## Supported Instructions
- [x] `+`: Increment the current data of current pointer by 1.
- [x] `-`: Decrement the current data of current pointer by 1.
- [x] `>`: Move the current pointer to the right.
- [x] `<`: Move the current pointer to the left.
- [ ] `[`: Loop start.
- [ ] `]`: Loop end.
- [ ] `.`: Output the byte at the data pointer.
- [ ] `,`: Accept one byte of input, storing its value in the byte at the data pointer.

## Extended Instructions
- [ ] `(`: Push the byte at the data pointer to the stack.
- [ ] `)`: Pop the byte at the data pointer to the stack.
- [ ] `@`: Absolute jump to the given address.
- [ ] `&`: System call with an index.

## LICENSE
Licensed under MIT License with ❤.
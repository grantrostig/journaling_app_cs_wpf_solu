#pragma once

class Junk {
    int i{};
public:
    int my_function( int i ){
        return i + 41;
    }
};

extern "C" {
    __declspec(dllexport) Junk* junk_create_instance() { // declspec(dllexport) is used to export the function from the DLL
        return new Junk(); // TODO??: handle memory management
    }
    __declspec(dllexport) int junk_add(Junk* junk, int i) { // TODO??: Is this LLM true?: linux equivalent: __attribute__((visibility("default")))
        return junk->my_function(i); 
    }
    __declspec(dllexport) void junk_destroy(Junk* junk) {
        delete junk; 
    }
}


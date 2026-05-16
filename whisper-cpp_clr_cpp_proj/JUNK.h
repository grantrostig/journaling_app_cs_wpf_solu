#pragma once

class Junk {
    int i{};
public:
    int my_function( int i ){
        return i + 41;
    }
};

extern "C" {
    __declspec(dllexport) Junk* junk_create_instance() {
        return new Junk(); // TODO??: handle memory management
    }
    __declspec(dllexport) int junk_add(Junk* junk, int i) {
        return junk->my_function(i); 
    }
    __declspec(dllexport) void junk_destroy(Junk* junk) {
        delete junk; 
    }
}


class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.2.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.2.0/tinycity-v3.2.0-osx-arm64"
      sha256 "a8864ec06aed0c0437fe941ec990337bb2455e6328a92b5068d5c8825fe7da0d"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.2.0/tinycity-v3.2.0-osx-x64"
      sha256 "4d3f91e045caac3f723aaf82445bdb1e52105990fb3524bb7672f8f8fc0cb967"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.2.0/tinycity-v3.2.0-linux-x64"
    sha256 "fc9362cc24fbbdaf29a4fd82fcf693abb77e1f5d7282553107953a78163feb5e"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.2.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.2.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.2.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end
















